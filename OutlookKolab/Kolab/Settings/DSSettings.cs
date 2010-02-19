using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OutlookKolab.Kolab.Settings
{
    public partial class DSSettings
    {
        public static DSSettings Load()
        {
            DSSettings settings = new DSSettings();
            if (File.Exists(Helper.SettingsPath))
            {
                settings.ReadXml(Helper.SettingsPath);
            }
            if (settings.Settings.Count == 0)
            {
                settings.Settings.AddSettingsRow("", "", "", "", "", "", "", "");
            }
            return settings;
        }

        public void Save()
        {
            Helper.EnsureStorePath();
            this.WriteXml(Helper.SettingsPath);
        }
    }
}
