using System;
using System.Linq;
using System.Xml;
using OutlookKolab.Kolab.Sync;
using System.IO;

namespace OutlookKolab.Kolab.Provider
{
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
            switch (type)
            {
                case LocalCacheProviderType.Contacts:
                    filename = Path.Combine(Helper.StorePath, "ContactsCache.xml");
                    break;
                case LocalCacheProviderType.Calendar:
                    filename = Path.Combine(Helper.StorePath, "CalendarCache.xml");
                    break;
            }

            cache = Load(filename);
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
                && Helper.Equals(entry.remoteChangedDate, message.LastModificationTime)
                && entry.remoteId == message.Subject;

            if (!result)
            {
                Log.d("syncisSame", "*********************** not equal ***********************");
                if (entry == null) Log.d("syncisSame", "entry == null");
                if (message == null) Log.d("syncisSame", "message == null");
                if (entry != null && message != null)
                {
                    if (!Helper.Equals(entry.remoteChangedDate, message.LastModificationTime))
                    {
                        Log.d("syncisSame", "getRemoteChangedDate="
                                + entry.remoteChangedDate.ToString("HH:mm:ss.fff") + ", getSentDate="
                                + message.LastModificationTime.ToString("HH:mm:ss.fff"));
                    }
                    if (entry.remoteId!=message.Subject)
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
