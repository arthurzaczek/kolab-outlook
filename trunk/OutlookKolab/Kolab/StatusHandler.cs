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

namespace OutlookKolab.Kolab
{
    using System;
    using System.Xml;
    using OutlookKolab.Kolab.Provider;
    using Outlook = Microsoft.Office.Interop.Outlook;

    /// <summary>
    /// Delegate for a simple sync notification
    /// </summary>
    public delegate void SyncNotifyHandler();
    /// <summary>
    /// Delegate for a sync status notification
    /// </summary>
    /// <param name="text">notification text</param>
    public delegate void SyncStatusHandler(string text);

    /// <summary>
    /// Async Status Dispatcher. SyncWorker will send status messages. Anyone can register those messages
    /// </summary>
    public class StatusHandler
    {
        /// <summary>
        /// Sync has started event
        /// </summary>
        public static event SyncNotifyHandler SyncStarted = null;
        /// <summary>
        /// Sync has finished event
        /// </summary>
        public static event SyncNotifyHandler SyncFinished = null;
        /// <summary>
        /// Sync status changed event
        /// </summary>
        public static event SyncStatusHandler SyncStatus = null;

        /// <summary>
        /// Send a sync status notification
        /// </summary>
        /// <param name="text">notification text</param>
        public static void writeStatus(String text)
        {
            Log.i("status", text);
            var temp = SyncStatus;
            if (temp != null)
            {
                temp(text);
            }
        }

        /// <summary>
        /// Send a sync finished notification
        /// </summary>
        public static void notifySyncFinished()
        {
            var temp = SyncFinished;
            if (temp != null)
            {
                temp();
            }
        }

        /// <summary>
        /// Send a sync start notification
        /// </summary>
        public static void notifySyncStarted()
        {
            var temp = SyncStarted;
            if (temp != null)
            {
                temp();
            }
        }
    }
}
