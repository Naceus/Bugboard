using System.Globalization;

namespace BugBoard.Api.Extensions;

public static class DateTimeDisplayExtensions
{
    private const string DisplayFormat = "dd.MM.yyyy HH:mm";

    private static readonly TimeZoneInfo DisplayTimeZone = ResolveDisplayTimeZone();
    private static readonly CultureInfo DisplayCulture = CultureInfo.GetCultureInfo("de-DE");

    public static string ToDisplayDateTime(this DateTime utcDateTime)
    {
        var normalizedUtcDateTime = utcDateTime.Kind switch
        {
            DateTimeKind.Utc => utcDateTime,
            DateTimeKind.Local => utcDateTime.ToUniversalTime(),
            _ => DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc)
        };

        var displayDateTime = TimeZoneInfo.ConvertTimeFromUtc(normalizedUtcDateTime, DisplayTimeZone);

        return displayDateTime.ToString(DisplayFormat, DisplayCulture);
    }

    public static string ToDisplayDateTime(this DateTime? utcDateTime)
    {
        return utcDateTime.HasValue
            ? utcDateTime.Value.ToDisplayDateTime()
            : "-";
    }

    private static TimeZoneInfo ResolveDisplayTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        }
    }
}
