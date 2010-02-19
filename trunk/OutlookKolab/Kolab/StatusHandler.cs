using System;
using outlook = Microsoft.Office.Interop.Outlook;
using System.Xml;
using System.Threading;
using OutlookKolab.Kolab.Provider;

namespace OutlookKolab.Kolab
{
    public delegate void SyncNotifyHandler();
    public delegate void SyncStatusHandler(string text);

    public class StatusHandler
    {
        public static event SyncNotifyHandler SyncStarted = null;
        public static event SyncNotifyHandler SyncFinished = null;
        public static event SyncStatusHandler SyncStatus = null;

        public static void writeStatus(String text)
        {
            Log.i("status", text);
            var temp = SyncStatus;
            if (temp != null)
            {
                temp.BeginInvoke(text, null, null);
            }
        }

        public static void notifySyncFinished()
        {
            var temp = SyncFinished;
            if (temp != null)
            {
                temp.BeginInvoke(null, null);
            }
        }

        public static void notifySyncStarted()
        {
            var temp = SyncStarted;
            if (temp != null)
            {
                temp.BeginInvoke(null, null);
            }
        }
    }
}
