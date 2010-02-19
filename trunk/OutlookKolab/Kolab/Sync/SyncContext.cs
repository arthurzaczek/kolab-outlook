using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;
using OutlookKolab.Kolab.Provider;

namespace OutlookKolab.Kolab.Sync
{
    public class SyncContext
    {
        public object LocalItem { get; set; }
        public DSLocalCache.CacheEntryRow CacheEntry { get; set; }
        public Outlook.MailItem Message { get; set; }
    }
}
