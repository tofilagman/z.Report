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
        IReportFeature Base64();
        bool Base64Content { get; }
        IReportFeature Landscape();
        
    }
}
