namespace Maria_Radio.Misc
{
    public struct SettingKeys
    {
        public struct General
        {
            public const string RunAtStartUp = "bRunAtStartUp";
            public const string MinimizeToTray = "bMinimizeToTray";
            public const string AlwaysOnTop = "bAlwaysOnTop";
            public const string ShowPopup = "bShowPopup";
            public const string NotifyProgram = "bNotifyProgram";
            public const string CheckUpdates = "bCheckUpdates";
        }

        public struct Record
        {
            public const string RecordPath = "strRecordPath";
            public const string AfterRecordOpenFolder = "bAfterRecordOpenFolder";
        }

        public struct Proxy
        {
            public const string UseProxy = "bUseProxy";
            public const string Host = "strHost";
            public const string Port = "iPort";
            public const string User = "strUser";
            public const string Pass = "strPass";
        }

        public struct Program
        {
            public const string Volume = "iVolume";
            public const string ShowPrograms = "bShowPrograms";
            public const string Positon = "ptPosition";
        }
    }
}