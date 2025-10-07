namespace VueApp1.Server.Models
{
    public class Tools
    {
        public static DateTime ConvertToDateTime(string dateString)
        {
            // 嘗試解析多種日期格式
            string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "MM/dd/yyyy" };
            if (DateTime.TryParseExact(dateString, formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime date))
            {
                return date;
            }
            else
            {
                throw new FormatException("錯誤的日期格式");
            }
        }
    }
}
