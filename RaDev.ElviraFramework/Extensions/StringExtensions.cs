using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using RaDev.ElviraFramework.Enums;

namespace RaDev.ElviraFramework.Extensions
{
    /// <summary>
    /// ���������� ��� ������ �� ��������
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// ������������� ������ � ������
        /// </summary>
        /// <param name="phrase">������</param>
        /// <param name="cases">������</param>
        /// <returns>������</returns>
        public static string CaseFormat(this string phrase, StringCase cases)
        {
            var splittedPhrase = phrase.Split(' ', '-', '.');
            var sb = new StringBuilder();

            switch (cases)
            {
                case StringCase.CamelCase:
                    sb.Append(splittedPhrase[0].ToLower());
                    splittedPhrase[0] = string.Empty;
                    break;
                case StringCase.PascalCase:
                    sb = new StringBuilder();
                    break;
            }

            foreach (var splittedPhraseChars in splittedPhrase.Select(s => s.ToCharArray()))
            {
                if (splittedPhraseChars.Length > 0)
                {
                    splittedPhraseChars[0] = ((new String(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
                }
                sb.Append(new String(splittedPhraseChars));
            }
            return sb.ToString();
        }

        /// <summary>
        /// �������� ������� � ������ �� ���������� ������� ��� �������
        /// </summary>
        /// <param name="phrase">�������� ������</param>
        /// <param name="sybmol">������ ������</param>
        /// <returns>������ ����� ������</returns>
        public static string ReplaceNotLetterOrDigit(this string phrase, char sybmol)
        {
            return phrase
                .Aggregate("", (current, currentChar) => current + (Char.IsLetterOrDigit(currentChar) ? currentChar : sybmol));
        }

        #region ��������� ������

        /// <summary>
        /// �������������� ������
        /// </summary>
        /// <param name="str">������</param>
        /// <returns>���������������� ������</returns>
        public static string Compress(this string str)
        {
            var byteArray = Encoding.UTF8.GetBytes(str);
            using (var stream = new MemoryStream())
            {
                using (var zip = new GZipStream(stream, CompressionMode.Compress))
                {
                    zip.Write(byteArray, 0, byteArray.Length);
                }
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        /// <summary>
        /// ��������������� ������
        /// </summary>
        /// <param name="str">���������������� ������</param>
        /// <returns>������</returns>
        public static string Decompress(this string str)
        {
            var bytes = Convert.FromBase64String(str);
            using (var stream = new MemoryStream(bytes))
            using (var zip = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new StreamReader(zip))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}