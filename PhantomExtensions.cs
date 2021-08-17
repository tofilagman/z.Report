using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.Report.Options;

namespace z.Report
{
    public static class PhantomExtensions
    {
        public static string GetValue(this DimensionUnits dimensionUnit)
        {
            switch (dimensionUnit)
            {
                case DimensionUnits.Millimeter:
                    return "mm";
                case DimensionUnits.Centimeter:
                    return "cm";
                case DimensionUnits.Inch:
                    return "in";
                default:
                case DimensionUnits.Pixel:
                    return "px";
            }
        }

        public static string GetValue(this Orientation? orientation)
        {
            switch (orientation)
            {
                case Orientation.Landscape:
                    return "landscape";
                default:
                case Orientation.Portrait:
                    return "portrait";
            }
        }
    }
}
