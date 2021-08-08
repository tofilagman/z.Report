using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Report
{
    public static class ReportBuilderExtensions
    {
        public static IApplicationBuilder UseReport(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReportMiddleware>();
        }
    }
}
