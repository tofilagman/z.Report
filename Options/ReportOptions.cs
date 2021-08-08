using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Report.Options
{
    public class ReportOptions
    {
        public ReportEngine Engine { get; set; } = ReportEngine.PhantomPdf;
        public string ReportPath { get; set; } = "zReport";
        public bool UseRelativePath {  get; set; } = true;
    }

    public enum ReportEngine
    {
        WkhtmlToPdf = 1,
        PhantomPdf = 2
    }
}
