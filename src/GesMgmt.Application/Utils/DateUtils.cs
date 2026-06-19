
namespace GesMgmt.Application.Utils
{
    public class DateUtils
    {
        /// <summary>
        /// Convierte un string con formato de fecha y hora ISO a DateTime.
        /// </summary>
        /// <param name="dateTimeString">String que representa la fecha y hora (ej: "2026-04-09 19:54:49.880").</param>
        /// <returns>DateTime si la conversión es exitosa; null si falla.</returns>
        public static DateTime? ConvertIsoStringToDateTime(string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
            {
                return null;
            }

            string[] formats = 
            {
                "yyyy-MM-dd HH:mm:ss.fff",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss"
            };

            if (DateTime.TryParseExact(dateTimeString, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            return null;
        }

    }
}