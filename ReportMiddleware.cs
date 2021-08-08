using z.Report.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using z.Data;

namespace z.Report
{
    public class ReportMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRenderService _renderService;

        public ReportMiddleware(RequestDelegate next, IRenderService renderService)
        {
            _next = next;
            _renderService = renderService;
        }

        public async Task Invoke(HttpContext context)
        {
            // put to the Response.Body our own stream so we can later forward the underlying buffer to jsreport
            var buffer = new MemoryStream();
            var originalResponseStream = context.Response.Body;
            context.Response.Body = buffer;

            context.Features.Set<IReportFeature>(new ReportFeature(context));

            try
            {
                await _next(context);

                var feature = context.Features.Get<IReportFeature>();

                // we don't deal with failed request and let the other middlewares handle it
                if (!feature.Enabled || context.Response.StatusCode != 200)
                {
                    buffer.Seek(0, SeekOrigin.Begin);
                    await buffer.CopyToAsync(originalResponseStream);
                    return;
                }

                buffer.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(buffer);
                string responseBody = await reader.ReadToEndAsync();

                feature.RenderRequest.Template = responseBody;

                var report = await _renderService.RenderAsync(feature.RenderRequest);
                //feature.AfterRender?.Invoke(report);

                context.Response.Headers["Content-Disposition"] = feature.RenderRequest.Options.ContentType();  

                if (feature.Base64Content)
                {
                    context.Response.ContentType = "application/pdf;base64";
                    var rpts = new StringContent($"data:application/pdf;base64,{ Convert.ToBase64String(report) }");
                    await rpts.CopyToAsync(originalResponseStream);
                }
                else
                {
                    context.Response.ContentType = feature.RenderRequest.Options.ContentType();
                    using var ms = new MemoryStream(report);
                    await ms.CopyToAsync(originalResponseStream);
                }
            }
            finally
            {
                // finally copy the jsreport output stream to the Response.Body
                context.Response.Body = originalResponseStream;
            }
        }
    }
}
