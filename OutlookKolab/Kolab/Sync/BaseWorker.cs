using System;
using outlook = Microsoft.Office.Interop.Outlook;
using System.Xml;
using System.Threading;

namespace OutlookKolab.Kolab.Sync
{
    public abstract class BaseWorker
    {
        private static Thread thread;
        protected outlook.Application app;

        private static String SYNC_ROOT = "SYNC";

        private static bool _isRunning = false;
        public static bool IsRunning
        {
            get
            {
                lock (SYNC_ROOT)
                {
                    return _isRunning;
                }
            }
        }

        private static bool _isStopping = false;
        public static bool IsStopping
        {
            get
            {
                lock (SYNC_ROOT)
                {
                    return _isStopping;
                }
            }
        }

        public BaseWorker(outlook.Application app)
        {
            this.app = app;
        }

        public void Start()
        {
            lock (SYNC_ROOT)
            {
                if (_isRunning) return;
                _isRunning = true;
                _isStopping = false;
                thread = new Thread(new ThreadStart(RunInternal));
                thread.Start();
            }
        }

        public static void Stop()
        {
            Thread tmp;
            lock (SYNC_ROOT)
            {
                _isStopping = true;
                tmp = thread;
            }
            if (tmp != null) tmp.Join(2000);
        }

        private void RunInternal()
        {
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                Log.e("worker", ex.ToString());
            }
            lock (SYNC_ROOT)
            {
                _isRunning = false;
                _isStopping = false;
            }
        }

        protected abstract void Run();
    }
}