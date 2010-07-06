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
    
    /// <summary>
    /// Local cache provider type
    /// </summary>
    public enum LocalCacheProviderType
    {
        /// <summary>
        /// Contacts cache provider
        /// </summary>
        Contacts,
        /// <summary>
        /// Calendar cache provider
        /// </summary>
        Calendar,
    }

    /// <summary>
    /// Implements the local cache provider
    /// </summary>
    public class LocalCacheProvider
        : IDisposable
    {
        /// <summary>
        /// Dataset cache
        /// </summary>
        DSLocalCache cache;
        /// <summary>
        /// Datasets filename
        /// </summary>
        string filename;

        /// <summary>
        /// Creates a new local cache provider
        /// </summary>
        /// <param name="type">cache provider type</param>
        public LocalCacheProvider(LocalCacheProviderType type)
        {
            filename = GetFileName(type);
            cache = Load(filename);
        }

        /// <summary>
        /// Returns the caches provider dataset filename based on the type
        /// </summary>
        /// <param name="type">type of the cache provider</param>
        /// <returns>Full filename an path</returns>
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

        /// <summary>
        /// Deletes the given provider type Dataset file
        /// </summary>
        /// <param name="type">type of the cache provider</param>
        public static void Delete(LocalCacheProviderType type)
        {
            var filename = GetFileName(type);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        /// <summary>
        /// Loads the Dataset file
        /// </summary>
        /// <param name="filename">Full filename and path to the Dataset file</param>
        /// <returns>Dataset</returns>
        private static DSLocalCache Load(string filename)
        {
            DSLocalCache cache = new DSLocalCache();
            FileTransaction.FixBrokenTransaction(filename);
            if (File.Exists(filename))
            {
                cache.ReadXml(filename);

                // Clean up invalid items
                // These are items created during a error state
                var invalidItems = cache.CacheEntry.Where(i => i.IsremoteIdNull() || i.IslocalIdNull() || i.IslocalHashNull() || i.IsremoteChangedDateNull()).ToList();
                foreach (var i in invalidItems)
                {
                    i.Delete();
                }
                cache.AcceptChanges();
            }
            return cache;
        }

        /// <summary>
        /// Saves the current cache provider dataset
        /// </summary>
        public void Save()
        {
            Helper.EnsureStorePath();
            using (var tx = new FileTransaction(filename))
            {
                cache.WriteXml(tx.FullTempFileName);
                tx.Commit();
            }
        }

        /// <summary>
        /// Returns the local cache entry for the given remote id.
        /// </summary>
        /// <param name="remoteId">remote id.</param>
        /// <returns>cache entry row</returns>
        public DSLocalCache.CacheEntryRow getEntryFromRemoteId(string remoteId)
        {
            return cache.CacheEntry.FirstOrDefault(i => i.remoteId == remoteId);
        }

        /// <summary>
        /// Returns the local cache entry for the given local id.
        /// </summary>
        /// <param name="localId">local id</param>
        /// <returns>cache entry row</returns>
        public DSLocalCache.CacheEntryRow getEntryFromLocalId(string localId)
        {
            return cache.CacheEntry.FirstOrDefault(i => i.localId == localId);
        }

        /// <summary>
        /// Deletes the given cache entry
        /// </summary>
        /// <param name="entry">Cache entry row to delete</param>
        public void deleteEntry(DSLocalCache.CacheEntryRow entry)
        {
            cache.CacheEntry.RemoveCacheEntryRow(entry);
        }

        /// <summary>
        /// Creates a new cache entry row and addes it to the dataset
        /// </summary>
        /// <returns>new cache entry row</returns>
        public DSLocalCache.CacheEntryRow createEntry()
        {
            return cache.CacheEntry.AddCacheEntryRow("", 0, DateTime.MinValue, "", "", "");
        }

        /// <summary>
        /// Checks if the cache entry and mail message are representing an item with the same content.
        /// </summary>
        /// <param name="entry">Local cache entry</param>
        /// <param name="message">Mail message</param>
        /// <returns>true if both are representing an item with the same content.</returns>
        public static bool isSame(DSLocalCache.CacheEntryRow entry, Microsoft.Office.Interop.Outlook.MailItem message)
        {
            // Do the check
            bool result = entry != null && message != null
                && entry.remoteChangedDate == message.GetChangedDate()
                && entry.remoteId == message.Subject;

            // If not equal print out some debug information
            if (!result)
            {
                Log.d("syncisSame", "*********************** not equal ***********************");
                if (entry == null) Log.d("syncisSame", "entry == null");
                if (message == null) Log.d("syncisSame", "message == null");
                if (entry != null && message != null)
                {
                    if (entry.remoteChangedDate != message.GetChangedDate())
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

        #region IDisposable Members

        /// <summary>
        /// IDispose implementaion
        /// </summary>
        public void Dispose()
        {
            if (cache != null)
            {
                cache.Dispose();
                cache = null;
            }
        }

        #endregion
    }
}
