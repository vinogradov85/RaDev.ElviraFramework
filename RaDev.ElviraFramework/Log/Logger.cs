using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using RaDev.ElviraFramework.Enums;

namespace RaDev.ElviraFramework.Log
{
    /// <summary>
    /// Лог в файл
    /// </summary>
    public static class Logger
    {
        #region Переменные

        /// <summary>
        /// Шлобальный путь хранения логов
        /// </summary>
        private static readonly string FileDir = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Список логеров
        /// </summary>
        private static readonly Dictionary<string, FileLogger> Tbl = new Dictionary<string, FileLogger>();

        /// <summary>
        /// Минимальный приоритет записи в лог
        /// </summary>
        public static LogWritePriority Priority = LogWritePriority.Medium;

        /// <summary>
        /// Запись лога в один файл
        /// </summary>
        public static bool InOneFile = false;

        /// <summary>
        /// Приоритет для записи исключений
        /// </summary>
        public static LogWritePriority PriorityForExceptions = LogWritePriority.High;

        /// <summary>
        /// Приоритет для логирования по умолчанию
        /// </summary>
        public static LogWritePriority DefaultPriority = LogWritePriority.Insignificant;

        #endregion

        #region Приватные методы

        /// <summary>
        /// Получить логер метода
        /// </summary>
        /// <returns>Логгер</returns>
        private static FileLogger GetLogger(string name = null)
        {
            var stackTrace = new StackTrace().GetFrame(2).GetMethod();
            var logName =
                InOneFile ? "" : name ?? stackTrace.ReflectedType.Name
                + "_" + DateTime.Now.ToString("yyyy_MM_dd");

            FileLogger result;
            if (!Tbl.TryGetValue(logName, out result))
            {
                result = new FileLogger(Path.Combine(FileDir, logName + ".log"));
                Tbl.Add(logName, result);
            }
            return result;
        }

        #endregion

        #region Запись в лог

        /// <summary>
        /// Записать объекта в лог
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <param name="priority">Приоритет записи в лог</param>
        public static void Write(object obj, LogWritePriority priority)
        {
            if (priority < Priority) return;
            //TODO Для объектов через рефлектор
            var info = obj.ToString();
            GetLogger().Write(info);
        }

        /// <summary>
        /// Записать объекта в лог
        /// </summary>
        /// <param name="obj">Объект</param>
        public static void Write(object obj)
        {
            Write(obj, obj is Exception ? PriorityForExceptions : DefaultPriority);
        }

        #endregion
    }
}
