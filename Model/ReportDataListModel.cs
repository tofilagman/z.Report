using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Report.Model
{
    public class ReportDataListModel<TData, TReportParameter>
    {
        public List<TData> ResultSet { get; set; }
        public TReportParameter Parameters { get; set; }
    }
}
