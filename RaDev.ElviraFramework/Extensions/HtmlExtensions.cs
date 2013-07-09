using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace RaDev.ElviraFramework.Extensions
{
    /// <summary>
    /// Расширения методов вывода
    /// </summary>
    public static class HtmlExtensions
    {
        #region Вывод данных

        #region Безопасность

        /// <summary>
        /// Преобразовать строку в "безопасную"
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>"Безопасная" строка</returns>
        public static MvcHtmlString SecureString(this string str)
        {
            return MvcHtmlString.Create(str);
        }

        /// <summary>
        /// Отключие экранирование вывода
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Строка</returns>
        public static HtmlString NoSecure(this string str)
        {
            return new HtmlString(str);
        }

        #endregion

        #region Длина строки

        /// <summary>
        /// Обрезать строку до нужного количества символов
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="length">Нужная длина</param>
        /// <returns>Строка</returns>
        public static string ForLength(this string str, int length)
        {
            var len = str.Length > length ? length : str.Length;
            return str.Substring(0, len);
        }

        /// <summary>
        /// Обрезать строку до нужного количества символов
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="length">Нужная длина</param>
        /// <returns>Строка</returns>
        public static HtmlString ForLength(this HtmlString str, int length)
        {
            var val = str.ToString();
            var len = val.Length > length ? length : val.Length;
            return new HtmlString(val.Substring(0, len));
        }

        #endregion

        #region Теги

        /// <summary>
        /// Удалить все теги из строки
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Строка</returns>
        public static string NoTags(this string str)
        {
            return Regex.Replace(str, @"<[^>]*>", string.Empty);
        }

        /// <summary>
        /// Удалить все теги из строки
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Строка</returns>
        public static string NoTags(this HtmlString str)
        {
            return str.ToString().NoTags();
        }

        /// <summary>
        /// Удалить тег скрипта
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Очищеная от скриптов строка</returns>
        public static string NoScript(this string str)
        {
            return Regex.Replace(str, @"<script\b[^>]*>(.*?)</script>", string.Empty);
        }

        /// <summary>
        /// Удалить тег скрипта
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Очищеная от скриптов строка</returns>
        public static string NoScript(this HtmlString str)
        {
            return str.ToString().NoScript();
        }

        #endregion

        #endregion
    }
}