using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using z.Report.Model;

namespace z.Report
{
    [MiddlewareFilter(typeof(ReportPipeline))]
    public abstract class ReportController<TReportParameter> : Controller where TReportParameter : class
    {
        protected IReportFeature ReportFeature { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ReportFeature = Configure();
            base.OnActionExecuting(context);
        }

        public virtual IReportFeature Configure()
        {
            return HttpContext.ReportFeature()
                 .Configure((req) => req.Options.PageOrientation = Options.Orientation.Portrait)
                 .Base64();
        }

        public virtual async Task<ReportDataListModel<TData, TReportParameter>> PostListProcess<TData>(List<TData> data)
        {
            if (data.Count == 0)
                throw new ReportNoDataException();

            return new ReportDataListModel<TData, TReportParameter>
            {
                ResultSet = data,
                Parameters = await BuildReportParameter()
            };
        }

        /// <summary>
        /// This is use when there were custom data for a report and or implementing multiple datasources
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<ReportDataModel<TData, TReportParameter>> PostProcess<TData>(TData data)
        { 
            return new ReportDataModel<TData, TReportParameter>
            {
                ResultSet = data,
                Parameters = await BuildReportParameter()
            };
        }

        public abstract Task<TReportParameter> BuildReportParameter();

    }
}
