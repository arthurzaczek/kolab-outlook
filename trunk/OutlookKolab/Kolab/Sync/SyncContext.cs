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

namespace OutlookKolab.Kolab.Sync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using OutlookKolab.Kolab.Provider;
    using Outlook = Microsoft.Office.Interop.Outlook;
    using System.Runtime.InteropServices;
    
    /// <summary>
    /// Represents the current item which is been syncing
    /// </summary>
    public class SyncContext
    {
        /// <summary>
        /// Current local item or null if not exists. Can only be null for new remote items.
        /// </summary>
        public object LocalItem { get; set; }
        /// <summary>
        /// Current cache entry or null if not exists. Can only be null for new remote items.
        /// </summary>
        public DSLocalCache.CacheEntryRow CacheEntry { get; set; }
        /// <summary>
        /// Current IMAP Message or null if not exists. Can only be null for new local items.
        /// </summary>
        public Outlook.MailItem Message { get; set; }

        /// <summary>
        /// Short text of the current local item used for the conflict dialog
        /// </summary>
        public string LocalItemText { get; set; }
        /// <summary>
        /// Short text of the current remote item used for the conflict dialog
        /// </summary>
        public string RemoteItemText { get; set; }

        public void ReleaseMessage()
        {
            if (Message != null)
            {
                Marshal.ReleaseComObject(Message);
                Message = null;
            }
        }
    }
}
