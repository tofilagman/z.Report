using z.Report.Drivers;
using System;

namespace z.Report.Options
{
    public class RenderOptions
    {
        public RenderOptions()
        {
            this.PageMargins = new Margins();
        }
        /// <summary>
        /// Sets the page size.
        /// </summary>
        [OptionFlag("-s")]
        public Size? PageSize { get; set; }

        /// <summary>
        /// Sets the page width in mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageHeight"/> has to be also specified.</remarks>
        [OptionFlag("--page-width")]
        public double? PageWidth { get; set; }

        /// <summary>
        /// Sets the page height in mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageWidth"/> has to be also specified.</remarks>
        [OptionFlag("--page-height")]
        public double? PageHeight { get; set; }

        /// <summary>
        /// Sets the page orientation.
        /// </summary>
        [OptionFlag("-O")]
        public Orientation? PageOrientation { get; set; }

        /// <summary>
        /// Sets the page margins.
        /// </summary>
        public Margins PageMargins { get; set; }
         
        public string ContentType()
        {
            return "application/pdf";
        }
         
        /// <summary>
        /// Indicates whether the PDF should be generated in lower quality.
        /// </summary>
        [OptionFlag("-l")]
        public bool IsLowQuality { get; set; }

        /// <summary>
        /// Number of copies to print into the PDF file.
        /// </summary>
        [OptionFlag("--copies")]
        public int? Copies { get; set; }

        /// <summary>
        /// Indicates whether the PDF should be generated in grayscale.
        /// </summary>
        [OptionFlag("-g")]
        public bool IsGrayScale { get; set; }

        /// <summary>
        /// Indicates whether the page can run JavaScript.
        /// </summary>
        [OptionFlag("-n")]
        public bool IsJavaScriptDisabled { get; set; }

        /// <summary>
        /// Minimum font size.
        /// </summary>
        [OptionFlag("--minimum-font-size")]
        public int? MinimumFontSize { get; set; }

        /// <summary>
        /// Use this if you need another switches that are not currently supported by Rotativa.
        /// </summary>
        [OptionFlag("")]
        public string CustomSwitches { get; set; }
         
        /// <summary>
        ///   Describe de DimensionUnit of the parameters Widht and Height.
        /// </summary>
        public DimensionUnits DimensionUnit { get; set; }

        /// <summary>
        ///   Format of the document available in generator.
        /// </summary>
        public Size Format { get; set; } = Size.A4;

        /// <summary>
        ///   Orientation which the document will be generated.
        /// </summary>
        public Orientation Orientation { get; set; } = Orientation.Portrait;

        /// <summary>
        ///   Define the scaling factor for the document. The default option
        ///   <c>1</c> represents 100%.
        /// </summary>
        public double ZoomFactor { get; set; } = 1d;
    }
}
