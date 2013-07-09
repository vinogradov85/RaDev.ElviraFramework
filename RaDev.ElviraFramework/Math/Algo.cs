using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RaDev.ElviraFramework.Math
{
    /// <summary>
    /// Расширения алгоритмических преобразований
    /// </summary>
    public static class Algo
    {
        /// <summary>
        /// Вычислить md5 для строки
        /// </summary>
        /// <param name="input">Строка</param>
        /// <returns>Строка Md5</returns>
        public static string Md5(this string input)
        {
            //Предотвращение падения из-за null
            var inputString = input ?? string.Empty;
            var md5Hasher = MD5.Create();
            // Преобразуем входную строку в массив байт и вычисляем хэш
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(inputString));
            var result = string.Join("", data.Select(item => item.ToString("x2")));
            return result;
        }
    }
}
