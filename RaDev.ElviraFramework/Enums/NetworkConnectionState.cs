using System;

namespace RaDev.ElviraFramework.Enums
{
    [Flags]
    public enum NetworkConnectionState : uint
    {
        InternetConnectionConfigured = 0x40,
        InternetConnectionLan = 0x02,
        InternetConnectionModem = 0x01,
        InternetConnectionModemBusy = 0x08,
        InternetConnectionOffline = 0x20,
        InternetConnectionProxy = 0x04,
        InternetRasInstalled = 0x10
    }
}
