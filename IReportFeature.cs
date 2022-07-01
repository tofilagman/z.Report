using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Report
{
    public interface IReportFeature
    {
        RenderRequest RenderRequest { get; set; }
        bool Enabled { get; }
        IReportFeature Disabled();
        IReportFeature Configure(Action<RenderRequest> req);
        HttpContext Context { get; set; }
        /// <summary>
        /// Use Base64 string response 
        /// </summary>
        /// <param name="appendBase64Prefix">Append PDF document string eg: data:application/pdf;base64,</param>
        /// <returns></returns>
        IReportFeature Base64(bool appendBase64Prefix = true);
        bool Base64Content { get; }
        bool Base64AppendString { get; }
        IReportFeature Landscape();
        
    }
}
