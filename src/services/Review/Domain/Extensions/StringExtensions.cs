namespace Domain.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///     Replaces one or more format items in a string with the string representation of a specified
    ///     object.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="format">A composite format string.</param>
    /// <returns>
    ///     A copy of format in which any format items are replaced by the string representation of
    ///     args.
    /// </returns>
    public static string Format(this string format, params object[] args)
    {
        return string.Format(format, args);
    }

    public static bool IsNullOrWhiteSpace(this string val)
    {
        return string.IsNullOrWhiteSpace(val);
    }
}
