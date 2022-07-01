using Microsoft.AspNetCore.Http;
using System;

namespace z.Report
{
    public class ReportFeature : IReportFeature
    {
        public ReportFeature(HttpContext context)
        {
            RenderRequest = new RenderRequest();
            Context = context;
            Enabled = true;
            Base64Content = false; 
        }

        public RenderRequest RenderRequest { get; set; }
        public bool Enabled { get; private set; }
        public HttpContext Context { get; set; }
        public bool Base64Content { get; private set; }
        public bool Base64AppendString { get; private set; }

        public IReportFeature Configure(Action<RenderRequest> req)
        {
            req.Invoke(RenderRequest);
            return this;
        }
         
        public IReportFeature Base64(bool appendBase64Prefix = true)
        {
            Base64Content = true;
            Base64AppendString = appendBase64Prefix;
            return this;
        }

        public IReportFeature Landscape()
        {
            RenderRequest.Options.PageOrientation = Options.Orientation.Landscape;
            return this;
        }

        /// <summary>
        /// This Option will disable the pdf rendering
        /// </summary>
        /// <returns></returns>
        public IReportFeature Disabled()
        {
            Enabled = false;
            return this;
        }
    }
}
