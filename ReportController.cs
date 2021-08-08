using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using z.Report.Model;

namespace z.Report
{
    public abstract class ReportController<TReportParameter> : Controller where TReportParameter : class
    {
        protected IReportFeature ReportFeature { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ReportFeature = Configure();
            base.OnActionExecuting(context);
        }
 
        private IReportFeature Configure()
        {
            return HttpContext.ReportFeature()
                 .Configure((req) => req.Options.PageOrientation = Options.Orientation.Portrait) 
                 .Base64();
        }

        public async Task<ReportDataModel<TData, TReportParameter>> PostProcess<TData>(List<TData> data)
        {
            if (data.Count == 0)
                throw new ReportNoDataException();

            return new ReportDataModel<TData, TReportParameter>
            {
                ResultSet = data,
                Parameters = await BuildReportParameter()
            };
        }

        public abstract Task<TReportParameter> BuildReportParameter();

    }
}
