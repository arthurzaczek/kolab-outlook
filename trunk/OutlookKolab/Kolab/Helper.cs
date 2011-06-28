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
    using OutlookKolab.Kolab.Sync;
    
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

        /// <summary>
        /// Exstract the Kolab XML Document from the given MailMessage
        /// </summary>
        /// <param name="message">Outlooks MailMessage item</param>
        /// <returns>Kolab XML String</returns>
        public static string ExtractXml(this Outlook.MailItem message)
        {
            if (message == null) throw new ArgumentNullException("message");

            // Take the first attachment
            // TODO: Look for the first attachment with current Kolab MimeType
            Outlook.Attachment a = message.Attachments.Cast<Outlook.Attachment>().FirstOrDefault();
            if (a != null)
            {
                // Get an IUnknown Pointer of that attachment
                IntPtr ptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(a.MAPIOBJECT);
                try
                {
                    // Call our little C++ helper to extract the attachment in memory
                    // Calling a.SaveAs(...) would lead to a bunch of troubles
                    // If all Attachments has the same name (like on a kolab server)
                    // then Outlook is only able to store 100 (yes! 100!) Attachments
                    // Why? Because it saves the attachment in a TempFolder (see Registry)
                    // Then it opens a FileSystemWatcher (or the Win32 API equivalent)
                    // But: If that filename already exists Outlook behaves like the Windows Explorer
                    // It creates kolab (1).xml, kolab (2).xml, ..., kolab (99).xml files
                    // Outlook is'nt able to save kolab (100).xml - I dont know why, 
                    // but here is our 100 Attachments limit
                    return OutlookKolabMAPIHelper.IMAPHelper.ReadAttachment(ptr);
                }
                finally
                {
                    // Release that COM Pointer
                    System.Runtime.InteropServices.Marshal.Release(ptr);
                }
            }
            else
            {
                // No Attachment found -> throw a SyncException an continue
                throw new SyncException(message.Subject, "Message " + message.Subject + " has not attachment");
            }
        }
    }
}
