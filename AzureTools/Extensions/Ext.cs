namespace JSCProjects.Azure.AzureTools.Extensions
{
    using System;
    using System.Globalization;

    public static class Ext
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss:ffffff Z";

        public static string ToWireFormattedString(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString(Format, CultureInfo.InvariantCulture);
        }

        public static DateTime ToUtcDateTime(this string wireFormattedString)
        {
            return DateTime.ParseExact(wireFormattedString, Format, CultureInfo.InvariantCulture).ToUniversalTime();
        }
    }
}
