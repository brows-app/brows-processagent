using System.Windows.Data;

namespace Brows.Windows.Data;

/// <summary>
/// Formats a date-time with a format string, taking both from bindings so that the text
/// is produced from both values whenever either of them is first available or changes.
/// </summary>
/// <remarks>
/// Values: a nullable <see cref="DateTime"/> and a format string. A null date-time produces
/// an empty string, and a null or empty format produces the general (<c>G</c>) format.
/// </remarks>
internal sealed class ProcessDateTimeConverter : IMultiValueConverter {
    private static string Format(DateTime value, string format, CultureInfo culture) {
        try {
            return value.ToString(string.IsNullOrEmpty(format) ? "G" : format, culture);
        }
        catch (FormatException) {
            return value.ToString(culture);
        }
    }

    /// <summary>
    /// Formats every month and weekday as a local time at the widest clock reading and keeps
    /// the longest result, so the text covers localized month and weekday names and the
    /// UTC offset that a local time adds to formats such as <c>O</c>.
    /// </summary>
    /// <remarks>
    /// Width is estimated by character count. That is exact for numeric formats and a close
    /// approximation for names in a proportional font.
    /// </remarks>
    private static string Widest(string format, CultureInfo culture) {
        var widest = string.Empty;
        for (var month = 1; month <= 12; month++) {
            // Seven consecutive days cover every weekday.
            for (var day = 22; day <= 28; day++) {
                var text = Format(new DateTime(2026, month, day, 12, 28, 28, System.DateTimeKind.Local), format, culture);
                if (text.Length > widest.Length) {
                    widest = text;
                }
            }
        }
        return widest;
    }

    /// <summary>
    /// Gets or sets whether to produce the widest text the format can produce instead of
    /// formatting the bound date-time.
    /// </summary>
    /// <value>
    /// True to format representative values so that a hidden element can reserve the width of
    /// the formatted text before the actual date-time is known.
    /// </value>
    public bool Sample { get; set; }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        var format = values is { Length: > 1 }
            ? values[1] as string
            : null;
        if (Sample) {
            return Widest(format, culture);
        }
        if (values is { Length: > 0 } && values[0] is DateTime value) {
            return Format(value, format, culture);
        }
        return string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        throw new NotSupportedException();
    }
}
