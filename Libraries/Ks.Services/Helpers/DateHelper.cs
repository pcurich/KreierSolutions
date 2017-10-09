using System;
 

namespace Ks.Services.Helpers
{
    public static class DateHelper
    { 
        public static String ToString(this DateTime? date, string format)
        {
            if (date.HasValue)
                return (new DateTime(date.Value.Year, date.Value.Month, date.Value.Day)).ToString(format);
            return "";
        }
    }
}