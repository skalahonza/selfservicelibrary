namespace SelfServiceLibrary.Web.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns --- if the string is null or empty
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DashEmpty(this string text) =>
            string.IsNullOrEmpty(text)
            ? "---"
            : text;
    }
}
