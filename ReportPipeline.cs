using Microsoft.AspNetCore.Builder;

namespace z.Report
{
    public class ReportPipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseReport();
        }
    }
}
