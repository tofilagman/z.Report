using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Report.Model
{
    public class ReportModel<TReportParameter> where TReportParameter : class
    {
        public string ReportName { get; set; }
        public string ReportFile { get; set; }
        public string UserId { get; set; }
        public DateTime CurrentDate { get; set; } = DateTime.Now.ToLocalTime();
        public TReportParameter ReportParameters { get; set; }
    }
}
