using System.Globalization;

namespace Effective_Mobile.Helpers
{
    /// <summary>
    /// Helper class for parsing orders from one line
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Extracts the parameter value from the line
        /// Example Number=1231ds; => 1231ds
        /// </summary>
        public static string Extract(string line, string parameter)
        {
            if (!line.Contains(parameter))
                return "";
            int parameterId = line.IndexOf(parameter);
            int semicolonId = line.IndexOf(';', parameterId);
            int equalId = line.IndexOf('=', parameterId);
            return line.Substring(equalId + 1, semicolonId - equalId - 1);
        }
        /// <summary>
        /// Parses string to the date
        /// </summary>
        /// <returns>Date in yyyy-MM-dd HH:mm:ss format</returns>
        public static DateTime ParseDeliveryTime(string line)
        {
            if (DateTime.TryParseExact(line, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out DateTime result))
            {
                return result.ToUniversalTime();
            }
            throw new FormatException($"Unable to parse DateTime from value: {line}");
        }
    }
}
