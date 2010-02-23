﻿/*
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
    
    public class SyncContext
    {
        public object LocalItem { get; set; }
        public DSLocalCache.CacheEntryRow CacheEntry { get; set; }
        public Outlook.MailItem Message { get; set; }

        public string LocalItemText { get; set; }
        public string RemoteItemText { get; set; }
    }
}