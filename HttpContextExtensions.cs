using Microsoft.AspNetCore.Http;

namespace z.Report
{
    public static class HttpContextExtensions
    {
        public static IReportFeature ReportFeature(this HttpContext context)
        {
            return context.Features.Get<IReportFeature>();
        }
         
    }
}
