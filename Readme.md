## z.Report

a library that wraps and render razor html to pdf

Engine Types:
	- WkhtmlToPdf: 1
	- PhantomPdf: 2

Initialize 
```c#

 services.AddReport((provider, options) =>
    { 
        options.Engine = ReportEngine.WkhtmlToPdf;
        options.ReportPath = "ReportData";
        options.UseRelativePath = true;
    });

```

When using the engine WkhtmlToPdf:
for windows;
 download the binary copy the bin files and save to relative path defined in report options
 https://wkhtmltopdf.org/downloads.html

 for linux:
    install the wkhtml from apt

```bash

apt-get update && apt-get install -y libgdiplus fontconfig xfonts-base
apt-get update && apt install -y wget xfonts-75dpi
     
cd /tmp && wget https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-1/wkhtmltox_0.12.6-1.buster_amd64.deb \
    && dpkg -i wkhtmltox_0.12.6-1.buster_amd64.deb \
    && apt -f install

```

Code Implementation

```c#

public class ReportParameters
    {
        public const string Unspecified = "(Unspecified)"; 
        public string Tag { get; set; }
    }
     
[MiddlewareFilter(typeof(ReportPipeline))]
public class ReportController : ReportController<ReportParameters>
    { 
        public override async Task<ReportParameters> BuildReportParameter()
        { 
            return new ReportParameters
            { 
                Tag = ReportParameters.Unspecified
            }; 
        }
         
        [Route("[action]"), HttpPost] 
        public async Task<IActionResult> Test()
        { 
            var rData = await ReportData.TestData();
            var pData = await PostProcess(rData);

            return PartialView("Test", pData);
        } 
    }

```

Sample Razor view

```razor

@using z.Report 
@model z.Report.Model.ReportDataModel<UserModel, ReportParameters>

<body>
    <h3>LIST OF USERS</h3>
    <div class="header" style="text-align:right">
        <div style="font-weight: bold">@Model.Parameters.Company</div>
        <div style="font-size: 11px">@Model.Parameters.Name (@Model.Parameters.Email)</div>
        <div style="font-size: 8px">@Model.Parameters.CurrentDate.FormatDate("ddd MMM dd yyyy hh:mm:ss")</div>
        <hr />
    </div>
    <div class="pga">

        <div class="columns">
            <table>
                <thead>
                    <tr>
                        <th width="10%">USERNAME</th>
                    </tr>
                </thead>
                @foreach (var itm in Model.ResultSet)
                {
                    <tr>
                        <td>@itm.UserName</td>
                    </tr>
                }
            </table>

        </div>
    </div>
</body>

```