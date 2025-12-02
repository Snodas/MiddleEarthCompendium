using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MiddleEarthCompendium.Converters
{
    public partial class DateFormatConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string dateString || string.IsNullOrEmpty(dateString))
                return value;

            var match = MonthDayYearRegex().Match(dateString);
            if (match.Success)
            {
                var month = match.Groups[1].Value;
                var dayYear = match.Groups[2].Value;

                var dayYearMatch = DayYearRegex().Match(dayYear);
                if (dayYearMatch.Success)
                {
                    var day = dayYearMatch.Groups[1].Value;
                    var year = dayYearMatch.Groups[2].Value;
                    var rest = match.Groups[3].Value;

                    return $"{month} {day}, {year}{rest}";
                }
            }

            return dateString;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        [GeneratedRegex(@"^(\w+)\s+(\d{5,6})(.*)$")]
        private static partial Regex MonthDayYearRegex();

        [GeneratedRegex(@"^(\d{1,2})(\d{3,4})$")]
        private static partial Regex DayYearRegex();
    }
}
