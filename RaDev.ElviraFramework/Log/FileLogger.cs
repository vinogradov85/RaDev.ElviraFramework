using System;
using System.Diagnostics;
using System.IO;

namespace RaDev.ElviraFramework.Log
{
    /// <summary>
    /// Логирование в файл
    /// </summary>
    public class FileLogger
    {
        #region Переменные

        /// <summary>
        /// Путь к файлу лога
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Объект синхронизации записи в лог
        /// </summary>
        private readonly object _synchroWriter = new object();

        #endregion

        /// <summary>
        /// Класс логирования
        /// </summary>
        /// <param name="filePath"></param>
        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        #region Запись в лог

        /// <summary>
        /// Запись информации в лог
        /// </summary>
        /// <param name="line">Информационная строка</param>
        public void Write(string line)
        {
            lock (_synchroWriter)
            {
                using (var logStreamWriter = File.AppendText(_filePath))
                {
                    logStreamWriter.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + "\t" + LogInvoker + "\t" + line);
                }
            }
        }

        #endregion

        /// <summary>
        /// Метод вызвавший логирование
        /// </summary>
        private static string LogInvoker
        {
            get
            {
                var stackTrace = new StackTrace().GetFrame(3).GetMethod();
                return string.Format("{0}::{1}"
                                     , stackTrace.ReflectedType.Name
                                     , stackTrace.Name);
            }
        }
    }
}
