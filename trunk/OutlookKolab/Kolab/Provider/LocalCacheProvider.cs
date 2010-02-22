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

namespace OutlookKolab.Kolab.Provider
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;

    using OutlookKolab.Kolab.Sync;
    
    public enum LocalCacheProviderType
    {
        Contacts,
        Calendar,
    }

    public class LocalCacheProvider
    {
        DSLocalCache cache;
        string filename;

        public LocalCacheProvider(LocalCacheProviderType type)
        {
            filename = GetFileName(type);
            cache = Load(filename);
        }

        private static string GetFileName(LocalCacheProviderType type)
        {
            switch (type)
            {
                case LocalCacheProviderType.Contacts:
                    return Path.Combine(Helper.StorePath, "ContactsCache.xml");
                case LocalCacheProviderType.Calendar:
                    return Path.Combine(Helper.StorePath, "CalendarCache.xml");
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public static void Delete(LocalCacheProviderType type)
        {
            var filename = GetFileName(type);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        private static DSLocalCache Load(string filename)
        {
            DSLocalCache cache = new DSLocalCache();
            if (File.Exists(filename))
            {
                cache.ReadXml(filename);
            }
            return cache;
        }

        public void Save()
        {
            Helper.EnsureStorePath();
            cache.WriteXml(filename);
        }

        public DSLocalCache.CacheEntryRow getEntryFromRemoteId(string remoteId)
        {
            return cache.CacheEntry.FirstOrDefault(i => i.remoteId == remoteId);
        }

        public DSLocalCache.CacheEntryRow getEntryFromLocalId(string localId)
        {
            return cache.CacheEntry.FirstOrDefault(i => i.localId == localId);
        }

        public void deleteEntry(DSLocalCache.CacheEntryRow entry)
        {
            cache.CacheEntry.RemoveCacheEntryRow(entry);
        }

        public DSLocalCache.CacheEntryRow createEntry()
        {
            return cache.CacheEntry.AddCacheEntryRow("", 0, DateTime.MinValue, "", "", "");
        }

        public static bool isSame(DSLocalCache.CacheEntryRow entry, Microsoft.Office.Interop.Outlook.MailItem message)
        {
            bool result = entry != null && message != null
                && Helper.Equals(entry.remoteChangedDate, message.GetChangedDate())
                && entry.remoteId == message.Subject;

            if (!result)
            {
                Log.d("syncisSame", "*********************** not equal ***********************");
                if (entry == null) Log.d("syncisSame", "entry == null");
                if (message == null) Log.d("syncisSame", "message == null");
                if (entry != null && message != null)
                {
                    if (!Helper.Equals(entry.remoteChangedDate, message.GetChangedDate()))
                    {
                        Log.d("syncisSame", "getRemoteChangedDate="
                                + entry.remoteChangedDate.ToString("HH:mm:ss.fff") + ", getSentDate="
                                + message.GetChangedDate().ToString("HH:mm:ss.fff"));
                    }
                    if (entry.remoteId != message.Subject)
                    {
                        Log.d("syncisSame", "getRemoteId=" + entry.remoteId
                                + ", getSubject=" + message.Subject);
                    }
                }
                Log.d("syncisSame", "*********************** not equal ***********************");
            }

            return result;
        }
    }
}
