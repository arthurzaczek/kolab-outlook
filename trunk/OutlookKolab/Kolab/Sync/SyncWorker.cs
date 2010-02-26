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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    using OutlookKolab.Kolab.Calendar;
    using OutlookKolab.Kolab.Constacts;
    using OutlookKolab.Kolab.Provider;
    using Outlook = Microsoft.Office.Interop.Outlook;

    /// <summary>
    /// Worker for synchronizing Outlook folder with Kolab folder
    /// </summary>
    public class SyncWorker : BaseWorker
    {
        /// <summary>
        /// Creates a new Sync Worker.
        /// </summary>
        /// <param name="app">Outlook Application Object</param>
        public SyncWorker(Outlook.Application app)
            : base(app)
        {
        }

        private static DSStatus.StatusEntryRow _status;
        /// <summary>
        /// Returnes the current status of a running SyncHandler. Returnes null if no Sync Handler is running. This happens during startup of a Sync Handler.
        /// </summary>
        /// <returns>A DSStatus.StatusEntryRow or null</returns>
        public static DSStatus.StatusEntryRow getStatus()
        {
            return _status;
        }

        /// <summary>
        /// Implements the Sync Worker
        /// </summary>
        protected override void Run()
        {
            // Notify that sync is starting
            StatusHandler.writeStatus("Starting Sync");
            StatusHandler.notifySyncStarted();

            // Load Status Dataset and Settings
            using (var dsStatus = DSStatus.Load())
            using (var settings = Settings.DSSettings.Load())
            {
                // Remember errors - used to update Status
                bool hasErrors = false;
                try
                {
                    // If stopsignal arrives return
                    if (IsStopping)
                    {
                        StatusHandler.writeStatus("Sync aborted");
                        return;
                    }

                    // Creates a new Contacts Handler
                    using (var handler = new SyncContactsHandler(settings, dsStatus, app))
                    {
                        if (shouldProcess(handler))
                        {
                            // Remember Status
                            _status = handler.getStatus();
                            // Start sync with current handler
                            sync(handler);
                            // Update error flag
                            hasErrors |= _status.errors > 0;
                            // Save status
                            dsStatus.Save();
                        }

                    }

                    // If stopsignal arrives return
                    if (IsStopping)
                    {
                        StatusHandler.writeStatus("Sync aborted");
                        return;
                    }

                    // Creates a new Calendar Handler
                    using (var handler = new SyncCalendarHandler(settings, dsStatus, app))
                    {
                        if (shouldProcess(handler))
                        {
                            // Remember Status
                            _status = handler.getStatus();
                            // Start sync with current handler
                            sync(handler);
                            // Update error flag
                            hasErrors |= _status.errors > 0;
                            // Save status
                            dsStatus.Save();
                        }
                    }

                    // Notify about sync has finished or errors
                    StatusHandler.writeStatus(hasErrors ? "Sync errors" : "Sync finished");
                }
                catch (Exception ex)
                {
                    // Very bad - report to user
                    StatusHandler.writeStatus("Sync error");
                    Helper.HandleError("Fatal error during sync", ex);
                }
                finally
                {
                    // Clear current status
                    _status = null;
                    dsStatus.Save();
                    StatusHandler.notifySyncFinished();
                }
            }
        }

        /// <summary>
        /// Checks if a sync handler should run. If both folder names are empty no sync should run.
        /// </summary>
        /// <param name="handler">Sync Handler to check</param>
        /// <returns>true if the handler should sync</returns>
        private bool shouldProcess(ISyncHandler handler)
        {
            return !string.IsNullOrEmpty(handler.GetIMAPFolderName()) && !string.IsNullOrEmpty(handler.GetOutlookFolderName());
        }

        /// <summary>
        /// Implements the sync algo.
        /// </summary>
        /// <param name="handler">current sync handler</param>
        private void sync(ISyncHandler handler)
        {
            // Get local cache provider
            LocalCacheProvider cache = handler.getLocalCacheProvider();

            // 1. retrieve list of all imap message headers
            StatusHandler.writeStatus("Fetching messages");
            try
            {
                Outlook.Folder imapFolder = null;
                using (var syncWait = new AutoResetEvent(false))
                {
                    // Get Imap Folder and mark for Outlook-Sync 
                    imapFolder = (Outlook.Folder)app.Session.GetFolderFromID(handler.GetIMAPFolderName(), handler.GetIMAPStoreID());
                    imapFolder.InAppFolderSyncObject = true;

                    // TODO: Do this for all IMAP Folder at once
                    // Creates a "Sync is ready" delegate
                    var del = new Microsoft.Office.Interop.Outlook.SyncObjectEvents_SyncEndEventHandler(delegate() { syncWait.Set(); });
                    try
                    {
                        // Starts Outlook-Sync
                        // If this is not happening outlook will use a cached version of the imap folder
                        app.Session.SyncObjects.AppFolders.SyncEnd += del;
                        app.Session.SyncObjects.AppFolders.Start();

                        // Wait at most 10 minutes for outlook
                        if (!syncWait.WaitOne(new TimeSpan(0, 10, 0))) throw new SyncException("IMAP", "Folder not sync");
                    }
                    finally
                    {
                        // This causes a nullref exception? 
                        //outlook.exe Error: 0 : generic: System.NullReferenceException: Object reference not set to an instance of an object.
                        //   at Microsoft.Office.Interop.Outlook.SyncObjectEvents_EventProvider.remove_SyncEnd(SyncObjectEvents_SyncEndEventHandler )
                        //   at Microsoft.Office.Interop.Outlook.SyncObjectClass.remove_SyncEnd(SyncObjectEvents_SyncEndEventHandler )
                        //   at OutlookKolab.Kolab.Sync.SyncWorker.sync(ISyncHandler handler) in P:\OutlookKolab\OutlookKolab\Kolab\Sync\SyncWorker.cs:line 141
                        //   at OutlookKolab.Kolab.Sync.SyncWorker.Run() in P:\OutlookKolab\OutlookKolab\Kolab\Sync\SyncWorker.cs:line 71
                        // app.Session.SyncObjects.AppFolders.SyncEnd -= del;
                    }
                }

                // cache processed entires
                Dictionary<string, bool> processedEntries = new Dictionary<string, bool>();
                // remember current status
                DSStatus.StatusEntryRow status = handler.getStatus();

                // Fetch all deleted entriy IDs
                // This is done by a little C++ managed helper as Outlook is not willing to tell me if an MailItem has been deleted or not
                ILookup<string, string> deletedEntryIDs = null;
                try
                {
                    // Saves an IUnknown Pointer for the current imap folder
                    IntPtr ptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(imapFolder.MAPIOBJECT);
                    try
                    {
                        // Fecht deleted entry IDs
                        deletedEntryIDs = OutlookKolapMAPIHelper.IMAPHelper.GetDeletedEntryIDs(ptr).ToLookup(i => i);
                    }
                    finally
                    {
                        // Release that pointer
                        System.Runtime.InteropServices.Marshal.Release(ptr);
                    }
                }
                catch (Exception ex)
                {
                    // oops
                    Log.e("imap", ex.ToString());
                    throw;
                }

                // Saves all conflics. Ask the user later.
                var conflictList = new List<SyncContext>();

                // Retreive a IMAP Message list and save this list.
                // We do not want to update this list during sync.
                // Messages will be created or deleted during sync.
                var msgList = imapFolder.Items.OfType<Outlook.MailItem>().ToList();
                foreach (var msg in msgList)
                {
                    // If stopsignal arrives return
                    if (IsStopping) return;

                    // Opens a new Sync Context
                    SyncContext sync = new SyncContext();
                    try
                    {
                        // Assing current message
                        sync.Message = msg;

                        // Report Status
                        StatusHandler.writeStatus(string.Format("Processing message {0}/{1}", status.incrementItems(), msgList.Count));

                        // if deleted -> continue
                        if (deletedEntryIDs.Contains(msg.EntryID))
                        {
                            Log.d("sync", "Found deleted IMAP Message, continue");
                            continue;
                        }

                        // 2. check message headers for changes
                        String subject = msg.Subject;
                        Log.d("sync", "2. Checking message " + subject);
                        // Check subject
                        if (string.IsNullOrEmpty(msg.Subject))
                        {
                            Log.d("sync", "Subject is empty - not a valid item. continue");
                            continue;
                        }

                        // 5. fetch local cache entry
                        sync.CacheEntry = cache.getEntryFromRemoteId(subject);

                        if (sync.CacheEntry == null)
                        {
                            // 6. found no local entry => must be a new one
                            Log.i("sync", "6. found no local entry => save");
                            status.incrementLocalNew();
                            // create a local item from server item
                            handler.createLocalItemFromServer(sync);
                            if (sync.CacheEntry == null)
                            {
                                // This indicates parsing errors
                                Log.w("sync", "createLocalItemFromServer returned a null object! See Logfile for parsing errors");
                            }

                        }
                        else
                        {
                            // Found a local cache item => server and local knows about this item
                            // do some more checks
                            Log.d("sync", "7. compare data to figure out what happened");
                            if (LocalCacheProvider.isSame(sync.CacheEntry, msg))
                            {
                                // Local cacheitem and server item are same => no changes on server made
                                Log.d("sync", "7.a/d cur=localdb");
                                if (handler.hasLocalItem(sync))
                                {
                                    // the item exists locally
                                    Log.d("sync", "7.a check for local changes");
                                    if (handler.hasLocalChanges(sync))
                                    {
                                        // The item has changed locally => udpate server item from local item
                                        Log.i("sync", "local changes found => updating ServerItem from Local");
                                        status.incrementRemoteChanged();
                                        handler.updateServerItemFromLocal(imapFolder, sync);
                                    }
                                }
                                else
                                {
                                    // local item is missing and no changes on the server where detected
                                    // It's save to delete the server item.
                                    Log.i("sync", "7.d entry missing => delete on server");
                                    status.incrementRemoteDeleted();
                                    handler.deleteServerItem(sync);
                                }
                            }
                            else
                            {
                                // Local cacheitem and server item are NOT same => changes where made on server
                                Log.d("sync", "7.b/c check for local changes and \"resolve\" the conflict");
                                if (handler.hasLocalChanges(sync))
                                {
                                    // Also local changes => conflict
                                    Log.i("sync", "local changes found: conflicting");
                                    status.incrementConflicted();

                                    // Get local ItemText - displayed in sync conflict dialogs list
                                    if (sync.LocalItem != null)
                                    {
                                        sync.LocalItemText = handler.GetItemText(sync);
                                    }
                                    else
                                    {
                                        sync.LocalItemText = "<deleted>";
                                    }

                                    // No idea what to write here. would need to parse XML, but thats only the text for the list.
                                    // Details are shown by the Dialog itself.
                                    sync.RemoteItemText = "remote changed";

                                    // Save conflicting item
                                    // USer will be asked later
                                    conflictList.Add(sync);
                                }
                                else
                                {
                                    // Item changed on server but not local => updating local item from server
                                    Log.i("sync", "no local changes found => updating local item from server");
                                    status.incrementLocalChanged();
                                    handler.updateLocalItemFromServer(sync);
                                }
                            }
                        }
                    }
                    catch (SyncException ex)
                    {
                        // Sync Exceptions are thrown by the handlers. This could be a parsing error or something else.
                        Log.e("sync", ex.ToString());
                        status.incrementErrors(ex);
                    }
                    finally
                    {
                        // Save local cache
                        cache.Save();
                    }

                    // Save message as processed - if it was not deleted
                    if (sync.CacheEntry != null && sync.CacheEntry.RowState != System.Data.DataRowState.Detached)
                    {
                        Log.d("sync", "8. remember message as processed (item id=" + sync.CacheEntry.localId + ")");
                        processedEntries[sync.CacheEntry.localId] = true;
                    }
                }

                // 9. for all unprocessed local items
                // 9.a upload/delete
                Log.d("sync", "9. process unprocessed local items");

                // Get a list of all local item IDs
                var items = handler.getAllLocalItemIDs().ToList();
                // Cache count
                int localItemsCount = items.Count();
                // init counter
                int currentLocalItemNo = 1;

                // Loop all local items
                foreach (var localId in items)
                {
                    // If stopsignal arrives return
                    if (IsStopping) return;

                    // Create a new sync context
                    SyncContext sync = new SyncContext();
                    try
                    {
                        Log.d("sync", "9. processing #" + localId);

                        // Notify progress
                        StatusHandler.writeStatus(String.Format("Processing local item {0}/{1}", currentLocalItemNo++, localItemsCount));

                        // continue if already processed 
                        if (processedEntries.ContainsKey(localId))
                        {
                            Log.d("sync", "9.a already processed from server: skipping");
                            continue;
                        }

                        // Get the local cache entry
                        sync.CacheEntry = cache.getEntryFromLocalId(localId);
                        if (sync.CacheEntry != null)
                        {
                            // There is a local cache entry
                            // We arrived that point - this means that no server item was found
                            // So: It must have been deleted on the server
                            Log.i("sync", "9.b found in local cache => deleting locally");
                            status.incrementLocalDeleted();
                            handler.deleteLocalItem(sync);
                        }
                        else
                        {
                            // There is no local cache entry. This means that this is a newly created local item
                            // Create a server item from local
                            Log.i("sync", "9.c not found in local cache => creating on server");
                            status.incrementRemoteNew();
                            handler.createServerItemFromLocal(imapFolder, sync, localId);
                        }
                    }
                    catch (SyncException ex)
                    {
                        // Sync Exceptions are thrown by the handlers. This could be a parsing error or something else.
                        Log.e("sync", ex.ToString());
                        status.incrementErrors(ex);
                    }
                    finally
                    {
                        // Save local cache
                        cache.Save();
                    }
                }

                // Conflict resolution
                if (conflictList.Count > 0)
                {
                    DlgConflictDialog.Show(handler, imapFolder, conflictList);
                }
            }
            finally
            {
                // Save local cache
                cache.Save();
            }
        }
    }
}