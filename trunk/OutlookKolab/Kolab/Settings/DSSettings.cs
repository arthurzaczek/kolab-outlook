/*
 * Copyright 2010 Arthur Zaczek <arthur@dasz.at>, dasz.at OG; All rights reserved.
 * Copyright 2010 David Schmitt <david@dasz.at>, dasz.at OG; All rights reserved.
 *
 *  This file is part of Kolab Sync for Outlook.

 *  Kolab Sync for Outlook is free software: you can redistribute it
 *  and/or modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation, either version 3 of
 *  the License, or (at your option) any later version.

 *  Kolab Sync for Outlook is distributed in the hope that it will be
 *  useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 *  of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  General Public License for more details.

 *  You should have received a copy of the GNU General Public License
 *  along with Kolab Sync for Outlook.  If not, see <http://www.gnu.org/licenses/>.
 */

namespace OutlookKolab.Kolab.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public partial class DSSettings
    {
        public static DSSettings Load()
        {
            DSSettings settings = new DSSettings();
            FileTransaction.FixBrokenTransaction(Helper.SettingsPath);
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
            using (var tx = new FileTransaction(Helper.SettingsPath))
            {
                this.WriteXml(tx.FullTempFileName);
                tx.Commit();
            }
        }
    }
}
