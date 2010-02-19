using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace OutlookKolab.Kolab
{
    public static class Helper
    {
        public static readonly string StorePath;
        public static readonly string SettingsPath;
        public static readonly string StatusPath;

        static Helper()
        {
            StorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"dasz.at\OutlookKolab");
            SettingsPath = Path.Combine(StorePath, "Settings.xml");
            StatusPath = Path.Combine(StorePath, "Status.xml");
        }

        public static void EnsureStorePath()
        {
            if (!Directory.Exists(StorePath))
                Directory.CreateDirectory(StorePath);
        }

        public static bool Equals(DateTime a, DateTime b)
        {
            return Math.Abs(a.Subtract(b).TotalMilliseconds) < 1000.0;
        }

        public static bool IsValid(this DateTime dt)
        {
            return dt.Year > 1900 && dt.Year < 2500;
        }
    }
}
