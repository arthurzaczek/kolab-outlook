/*
 * Copyright 2010 Arthur Zaczek <arthur@dasz.at>, dasz.at OG; All rights reserved.
 * Copyright 2010 David Schmitt <david@dasz.at>, dasz.at OG; All rights reserved.
 *
 *  This file is part of Kolab Sync for Outlook.
using System.Windows.Forms;

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

namespace OutlookKolab.Kolab
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    
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

        public static void HandleError(Exception ex)
        {
            HandleError("Error", ex);
        }

        public static void HandleError(string caption, Exception ex)
        {
            Log.e("generic", ex.ToString());
            MessageBox.Show(ex.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
