using System;

namespace z.Report
{
    public static class ReportViewExtensions
    {
        public static decimal FormatNumber(this object field, int decPoint = 2)
        {
            if (field == null)
                return 0;

            return decimal.Round(Convert.ToDecimal(field), decPoint, MidpointRounding.AwayFromZero);
        }

        public static string FormatDate(this object field, string format)
        {
            if (field == null)
                return string.Empty;

            return FormatDate(Convert.ToDateTime(field), format);
        }

        public static string FormatDate(this DateTime? dateTime, string format)
        {
            if (dateTime == null)
                return string.Empty;

            return dateTime?.ToString(format);
        }

        public static string IfTrue(this object field, string value)
        {
            if (field == null)
                return string.Empty;

            return Convert.ToBoolean(field) ? value : string.Empty;
        }

        public static string IfCondition(this object field, object compVal, string value)
        {
            if (field == null)
                return string.Empty;

            return field.ToString().Equals(compVal.ToString(), StringComparison.InvariantCultureIgnoreCase) ? value : string.Empty;
        }

        public static string IfGreaterThan(this object field, int compVal, string value)
        {
            if (field == null)
                return string.Empty;

            return Convert.ToInt32(field) > compVal ? value : string.Empty;
        }

        public static double ToDouble(this object field)
        {
            if (field == null)
                return 0;

            return Convert.ToDouble(field);
        }

        public static double Add(this object field, double a, double b)
        {
            if (field == null)
                return 0;
            field = a + b;
            return Convert.ToDouble(field);
        }

        public static double Subract(this object field, double a, double b)
        {
            if (field == null)
                return 0;
            field = a - b;
            return Convert.ToDouble(field);
        }

        public static double Multiply(this object field, double a, double b)
        {
            if (field == null)
                return 0;
            field = a * b;
            return Convert.ToDouble(field);
        }

        public static double Divide(this object field, double a, double b)
        {
            if (field == null)
                return 0;
            field = a / b;
            return Convert.ToDouble(field);
        }

        public static double Modulo(this object field, double a, double b)
        {
            if (field == null)
                return 0;
            field = a % b;
            return Convert.ToDouble(field);
        }
    }
}
