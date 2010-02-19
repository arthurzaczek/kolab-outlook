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
    using System.Threading;
    using System.Xml;

    using Outlook = Microsoft.Office.Interop.Outlook;
    
    public abstract class BaseWorker
    {
        private static Thread thread;
        protected Outlook.Application app;

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

        public BaseWorker(Outlook.Application app)
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