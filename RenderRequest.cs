using z.Report.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Report
{
    public class RenderRequest
    {
        public RenderRequest()
        {
            this.Options = new RenderOptions();
        }

        public string Template { get; set; } 
        public RenderOptions Options { get; set; }
    }
}
