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
    using System.Windows.Forms;
    using Outlook = Microsoft.Office.Interop.Outlook;
    
    /// <summary>
    /// Static class with helper methods
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// StorePath cache
        /// </summary>
        public static readonly string StorePath;
        /// <summary>
        /// SettingsPath cache
        /// </summary>
        public static readonly string SettingsPath;
        /// <summary>
        /// StatusPath cache
        /// </summary>
        public static readonly string StatusPath;

        /// <summary>
        /// Static constructor
        /// fills path/filename caches
        /// </summary>
        static Helper()
        {
            StorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"dasz.at\OutlookKolab");
            SettingsPath = Path.Combine(StorePath, "Settings.xml");
            StatusPath = Path.Combine(StorePath, "Status.xml");
        }

        /// <summary>
        /// Ensurse that the storage path exists
        /// </summary>
        public static void EnsureStorePath()
        {
            if (!Directory.Exists(StorePath))
                Directory.CreateDirectory(StorePath);
        }

        /// <summary>
        /// Checks if a DateTime is valid. Valid means that year is between 1900 and 2500
        /// </summary>
        /// <param name="dt">DateTime to check</param>
        /// <returns>true of Year is between 1900 and 2500</returns>
        public static bool IsValid(this DateTime dt)
        {
            return dt.Year > 1900 && dt.Year < 2500;
        }

        /// <summary>
        /// Handles an exception. Shows a MessageBox.
        /// Exception will also be logged.
        /// </summary>
        /// <param name="ex">Exception to show</param>
        public static void HandleError(Exception ex)
        {
            HandleError("Error", ex);
        }

        /// <summary>
        /// Handles an exception. Shows a MessageBox.
        /// Exception will also be logged.
        /// </summary>
        /// <param name="caption">caption of the MessageBox shown</param>
        /// <param name="ex">Exception to show</param>
        public static void HandleError(string caption, Exception ex)
        {
            if (ex == null) { throw new ArgumentNullException("ex"); }

            Log.e("generic", ex.ToString());
            MessageBox.Show(ex.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Returns the changedate of an MailItem. First SentOn is inspected. If Emtpy, ReceivedTime is used.
        /// </summary>
        /// <param name="item">Outlook MailItem</param>
        /// <returns>ChangeDate</returns>
        public static DateTime GetChangedDate(this Outlook.MailItem item)
        {
            if (item == null) { throw new ArgumentNullException("item"); }

            DateTime result = item.SentOn;
            if (!result.IsValid())
            {
                result = item.ReceivedTime;
            }
            return result;
        }
    }
}
