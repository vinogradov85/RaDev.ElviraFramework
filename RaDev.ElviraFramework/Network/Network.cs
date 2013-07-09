using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using RaDev.ElviraFramework.Enums;

namespace RaDev.ElviraFramework.Network
{
    /// <summary>
    /// Работа с сетью
    /// </summary>
    public static class Network
    {
        #region Вспомогательные

        [DllImport("Wininet.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InternetGetConnectedState(out NetworkConnectionState lpdwFlags, uint dwReserved);

        #endregion

        #region Сеть

        /// <summary>
        /// Получить список сетевых адаптеров
        /// </summary>
        /// <returns>Список сетевых адаптеров</returns>
        public static NetworkInterface[] Adapters()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }

        /// <summary>
        /// Подключен ли компьютер к интернету
        /// </summary>
        /// <returns>Подключен</returns>
        public static bool InternetAvailable()
        {
            NetworkConnectionState flags;
            return InternetGetConnectedState(out flags, 0U)
                   &&
                   (flags & NetworkConnectionState.InternetConnectionConfigured) ==
                   NetworkConnectionState.InternetConnectionConfigured;
        }

        /// <summary>
        /// Пропинговать по IP адресу
        /// </summary>
        /// <param name="ip">Ip</param>
        /// <param name="timeout">Таймаут в милисекундах</param>
        /// <returns>Успешно</returns>
        public static bool Ping(IPAddress ip, int timeout = 500)
        {
            var ping = new Ping();
            var replyping = ping.Send(ip, timeout);
            return replyping != null && replyping.Status == IPStatus.Success;
        }

        /// <summary>
        /// Получить IP адрес по имени хоста или адресу
        /// </summary>
        /// <param name="name">Имя хоста или адрес</param>
        /// <param name="timeout">Таймаут в милисекундах</param>
        /// <returns>IP адрес</returns>
        public static IPAddress GetIpByHostNameOrAddress(string name, int timeout = 500)
        {
            var ping = new Ping();
            var replyping = ping.Send(name, timeout);
            return replyping == null ? IPAddress.None : replyping.Address;
        }

        #endregion

        #region Скачать

        /// <summary>
        /// Скачать данные по адресу
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <returns>Данные</returns>
        public static byte[] Download(Uri url)
        {
            using (var myWebClient = new WebClient())
            {
                return myWebClient.DownloadData(url);
            }
        }

        /// <summary>
        /// Скачать строку по адресу
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <returns>Строка</returns>
        public static string DownloadString(Uri url)
        {
            var bytes = Download(url);
            return Encoding.Default.GetString(bytes);
        }

        /// <summary>
        /// Скачать изображение
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <returns>Изображение</returns>
        public static Image DownloadImage(Uri url)
        {
            var bytes = Download(url);
            using (var stream = new MemoryStream(bytes))
                return Image.FromStream(stream);
        }

        #endregion
    }
}
