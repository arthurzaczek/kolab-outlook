//#define MULTI_THREADED

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
    
    /// <summary>
    /// Abstract base class for all workers. 
    /// Implements threading and locking.
    /// This class ensurses that only one worker is running at the same time.
    /// </summary>
    public abstract class BaseWorker
    {
        /// <summary>
        /// lock object
        /// </summary>
        private static readonly object _lock = new object();

#if MULTI_THREADED
        /// <summary>
        /// Worker Thread
        /// </summary>
        private static Thread thread;
#endif

        /// <summary>
        /// Saved Outlook Application Object
        /// </summary>
        protected Outlook.Application app;

        private static bool _isRunning = false;
        /// <summary>
        /// Returnes true if a worker is running. Only one worker can run a the same time
        /// </summary>
        public static bool IsRunning
        {
            get
            {
                lock (_lock)
                {
                    return _isRunning;
                }
            }
        }

        private static bool _isStopping = false;
        /// <summary>
        /// Returnes true if a worker is stopping.
        /// </summary>
        public static bool IsStopping
        {
            get
            {
                lock (_lock)
                {
                    return _isStopping;
                }
            }
        }

        /// <summary>
        /// Creates a new Worker.
        /// </summary>
        /// <param name="app">Outlook Application Object</param>
        public BaseWorker(Outlook.Application app)
        {
            this.app = app;
        }

        /// <summary>
        /// Starts the worker.
        /// If a worker is already running this method does nothing.
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning) return;
                _isRunning = true;
                _isStopping = false;
#if MULTI_THREADED
                thread = new Thread(new ThreadStart(RunInternal));
                thread.Start();
#else
                RunInternal();
#endif
            }
        }

        /// <summary>
        /// Stops a running worker. If no worker is running this method does nothing
        /// </summary>
        public static void Stop()
        {
#if MULTI_THREADED
            Thread tmp;
#endif
            lock (_lock)
            {
                if(_isRunning) _isStopping = true;
#if MULTI_THREADED
                tmp = thread;
#endif
            }
#if MULTI_THREADED
            if (tmp != null) tmp.Join(2000);
#endif
        }

        /// <summary>
        /// Internal Thread Run Method
        /// </summary>
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
            lock (_lock)
            {
                _isRunning = false;
                _isStopping = false;
            }
        }

        /// <summary>
        /// The Worker method.
        /// </summary>
        protected abstract void Run();
    }
}