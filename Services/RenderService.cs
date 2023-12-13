using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using z.Report.Options;
using Microsoft.Extensions.Hosting;
using z.Report.Drivers;
using ChromiumHtmlToPdfLib.Settings;
using ChromiumHtmlToPdfLib;
using z.Data;
using Microsoft.Extensions.Logging;

namespace z.Report.Services
{
    public class RenderService : IRenderService
    {
        private readonly ReportOptions Option;
        private readonly IHostEnvironment Environment;
        private readonly ILogger<RenderService> logger;

        public RenderService(ReportOptions option, IHostEnvironment environment, ILogger<RenderService> logger)
        {
            this.Option = option;
            this.Environment = environment;
            this.logger = logger;
        }

        public async Task<byte[]> RenderAsync(RenderRequest request)
        {
            switch (Option.Engine)
            {
                case ReportEngine.PhantomPdf:
                    return await RenderPhantomAsync(request);
                case ReportEngine.WkhtmlToPdf:
                    return await RenderwkHtmlToPdfAsync(request);
                case ReportEngine.Chrome:
                    return await RenderChromeAsync(request);
                default:
                    throw new InvalidOperationException("Invalid Engine");
            }
        }

        private async Task<byte[]> RenderwkHtmlToPdfAsync(RenderRequest request)
        {
            var WkhtmlPath = GetPath();
            return await Task.FromResult(PdfDriver.ConvertHtml(WkhtmlPath, this.GetConvertOptions(request.Options), request.Template));
        }

        private async Task<byte[]> RenderPhantomAsync(RenderRequest request)
        {
            var phantomRootFolder = GetPath();

            var outputFolder = Path.Combine(phantomRootFolder, "temp");
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            var htmlFileName = WriteHtmlToTempFile(phantomRootFolder, request.Template);

            try
            {
                var exeToUse = GetOsExecutableName();
                WriteResourcesToDisk(phantomRootFolder, exeToUse);
                WriteResourcesToDisk(phantomRootFolder, Consts.Rasterize);
                var mdf = ExecutePhantomJs(phantomRootFolder, exeToUse, htmlFileName, outputFolder, request.Options);
                return await Task.FromResult(mdf);
            }
            finally
            {
                File.Delete(Path.Combine(phantomRootFolder, htmlFileName));
            }
        }

        private async Task<byte[]> RenderChromeAsync(RenderRequest request)
        {
            var phantomRootFolder = GetPath();

            var outputFolder = Path.Combine(phantomRootFolder, "temp");
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            var htmlFileName = WriteHtmlToTempFile(outputFolder, request.Template, false);
            var stream = new MemoryStream();
            try
            {
                var pageSettings = new PageSettings(request.Options.Format.ToPaperFormat())
                {
                    Scale = request.Options.ZoomFactor,
                    Landscape = request.Options.PageOrientation == Orientation.Landscape, 
                };
                using var converter = new Converter { UseOldHeadlessMode = true };
                var uri = new ConvertUri(htmlFileName);
                 
                await converter.ConvertToPdfAsync(uri, stream, pageSettings, logger: logger);
                return stream.ToArray();
            }
            finally
            {
                //File.Delete(htmlFileName);
            }
        }

        /// <summary>
        /// Returns properties with OptionFlag attribute as one line that can be passed to PhantomPdf binary.
        /// </summary>
        /// <returns>Command line parameter that can be directly passed to PhantomPdf binary.</returns>
        protected virtual string GetConvertOptions(RenderOptions options)
        {
            var result = new StringBuilder();

            var fields = options.GetType().GetProperties();
            foreach (var fi in fields)
            {
                var of = fi.GetCustomAttributes(typeof(OptionFlag), true).FirstOrDefault() as OptionFlag;
                if (of == null)
                    continue;

                object value = fi.GetValue(options, null);
                if (value == null)
                    continue;

                if (fi.PropertyType == typeof(Dictionary<string, string>))
                {
                    var dictionary = (Dictionary<string, string>)value;
                    foreach (var d in dictionary)
                    {
                        result.AppendFormat(" {0} {1} {2}", of.Name, d.Key, d.Value);
                    }
                }
                else if (fi.PropertyType == typeof(bool))
                {
                    if ((bool)value)
                        result.AppendFormat(CultureInfo.InvariantCulture, " {0}", of.Name);
                }
                else
                {
                    result.AppendFormat(CultureInfo.InvariantCulture, " {0} {1}", of.Name, value);
                }
            }

            return result.ToString().Trim();
        }

        private static string GetOsExecutableName() =>
             RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Consts.WinEXE
             : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Consts.LinuxEXE
             : Consts.OSXEXE;

        private void WriteResourcesToDisk(string phantomRootFolder, string fileName)
        {
            var zipFileName = $"{Path.GetFileNameWithoutExtension(fileName)}.zip";
            var zipFilePath = Path.Combine(phantomRootFolder, zipFileName);
            var unzippedFilePath = Path.Combine(phantomRootFolder, fileName);

            if (File.Exists(zipFilePath) || File.Exists(unzippedFilePath)) return;

            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"z.Report.Resources.{zipFileName}";

            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            // using (var binaryReader = new BinaryReader(stream))
            using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
            using (var binaryWriter = new BinaryWriter(fileStream))
            {
                if (stream == null) return;

                var byteArray = new byte[stream.Length];
                stream.Read(byteArray, 0, byteArray.Length);
                binaryWriter.Write(byteArray);
            }

            ZipFile.ExtractToDirectory(zipFilePath, phantomRootFolder);
            File.Delete(zipFilePath);
        }

        private byte[] ExecutePhantomJs(string phantomRootFolder, string phantomJsExeToUse, string inputFileName, string outputFolder, RenderOptions param)
        {
            var outputFileName = Path.GetFileNameWithoutExtension(inputFileName);
            var outputFilePath = Path.Combine(outputFolder, $"{outputFileName}.pdf");
            try
            {
                var layout = param.PageWidth > 0 && param.PageHeight > 0
                  ? $"{param.PageWidth}{param.DimensionUnit.GetValue()}*{param.PageHeight}{param.DimensionUnit.GetValue()}"
                  : param.Format.ToString();

                var exePath = Path.Combine(phantomRootFolder, phantomJsExeToUse);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    SetFilePermission(phantomRootFolder, phantomJsExeToUse);

                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = phantomRootFolder,
                    Arguments = $@"rasterize.js ""{inputFileName}"" ""{outputFilePath}"" ""{layout}"" {param.ZoomFactor} ""{param.PageOrientation.GetValue()}""",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                if (Debugger.IsAttached)
                    Console.WriteLine(startInfo.Arguments);

                var proc = new Process { StartInfo = startInfo };
                proc.Start();
                proc.WaitForExit();

                return File.ReadAllBytes(outputFilePath);
            }
            finally
            {
                if (File.Exists(outputFilePath))
                    File.Delete(outputFilePath);
            }
        }

        private void SetFilePermission(string phantomRootFolder, string fileName)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                WorkingDirectory = phantomRootFolder,
                Arguments = $@"-c ""chmod +x {fileName}""",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var proc = new Process { StartInfo = startInfo };
            proc.Start();
            proc.WaitForExit();
        }

        private string WriteHtmlToTempFile(string phantomRootFolder, string html, bool filenameOnly = true)
        {
            var filename = $"{Guid.NewGuid()}.html";
            var absolutePath = Path.Combine(phantomRootFolder, filename);

            if (OperatingSystem.IsWindows())
            {
                absolutePath = absolutePath.Replace('/', '\\');
            }

            File.WriteAllText(absolutePath, html);

            return filenameOnly ? filename : absolutePath;
        }

        private string GetPath()
        {
            var wkhtmltopdfPath = Path.Combine(Environment.ContentRootPath, Option.ReportPath);

            if (!Option.UseRelativePath)
            {
                wkhtmltopdfPath = Option.ReportPath;
            }

            if (Option.Engine == ReportEngine.WkhtmlToPdf && !Directory.Exists(wkhtmltopdfPath) && OperatingSystem.IsWindows())
            {
                throw new ApplicationException("Folder containing wkhtmltopdf.exe not found, searched for " + wkhtmltopdfPath);
            }

            return wkhtmltopdfPath;
        }

    }
}
