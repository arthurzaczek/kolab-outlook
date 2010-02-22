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

    public class SyncWorker : BaseWorker
    {
        public SyncWorker(Outlook.Application app)
            : base(app)
        {
        }

        private static DSStatus.StatusEntryRow _status;
        public static DSStatus.StatusEntryRow getStatus()
        {
            return _status;
        }

        protected override void Run()
        {
            StatusHandler.writeStatus("Starting Sync");
            StatusHandler.notifySyncStarted();
            var dsStatus = DSStatus.Load();
            bool hasErrors = false;
            try
            {
                var settings = Settings.DSSettings.Load();
                ISyncHandler handler = null;

                if (IsStopping)
                {
                    StatusHandler.writeStatus("Sync aborted");
                    return;
                }

                handler = new SyncContactsHandler(settings, dsStatus, app);
                if (shouldProcess(handler))
                {
                    _status = handler.getStatus();
                    sync(handler);
                    hasErrors |= _status.errors > 0;
                    dsStatus.Save();
                }

                if (IsStopping)
                {
                    StatusHandler.writeStatus("Sync aborted");
                    return;
                }

                handler = new SyncCalendarHandler(settings, dsStatus, app);
                if (shouldProcess(handler))
                {
                    _status = handler.getStatus();
                    sync(handler);
                    hasErrors |= _status.errors > 0;
                    dsStatus.Save();
                }
                _status = null;

                StatusHandler.writeStatus(hasErrors ? "Sync errors" : "Sync finished");
            }
            catch (Exception ex)
            {
                Log.e("sync", ex.ToString());
                StatusHandler.writeStatus("Sync error");
                ShowErrorDialog(ex);
            }
            finally
            {
                dsStatus.Save();
                StatusHandler.notifySyncFinished();
            }
        }

        private bool shouldProcess(ISyncHandler handler)
        {
            return !string.IsNullOrEmpty(handler.GetIMAPFolderName());
        }

        private void sync(ISyncHandler handler)
        {
            StatusHandler.writeStatus("Fetching messages");
            LocalCacheProvider cache = handler.getLocalCacheProvider();

            try
            {
                // 1. retrieve list of all imap message headers
                var imapFolder = (Outlook.Folder)app.Session.GetFolderFromID(handler.GetIMAPFolderName(), handler.GetIMAPStoreID());

                Dictionary<string, bool> processedEntries = new Dictionary<string, bool>();
                DSStatus.StatusEntryRow status = handler.getStatus();

                ILookup<string, string> deletedEntryIDs = null;
                try
                {
                    IntPtr ptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(imapFolder.MAPIOBJECT);
                    try
                    {
                        deletedEntryIDs = OutlookKolapMAPIHelper.IMAPHelper.GetDeletedEntryIDs(ptr).ToLookup(i => i);
                    }
                    finally
                    {
                        System.Runtime.InteropServices.Marshal.Release(ptr);
                    }
                }
                catch (Exception ex)
                {
                    Log.e("imap", ex.ToString());
                    throw;
                }

                var conflictList = new List<SyncContext>();
                var msgList = imapFolder.Items.OfType<Outlook.MailItem>().ToList();
                foreach (var msg in msgList)
                {
                    if (IsStopping) return;

                    SyncContext sync = new SyncContext();
                    try
                    {
                        sync.Message = msg;

                        StatusHandler.writeStatus(string.Format("Processing message {0}/{1}", status.incrementItems(), msgList.Count));

                        if (deletedEntryIDs.Contains(msg.EntryID))
                        {
                            Log.d("sync", "Found deleted IMAP Message, continue");
                            continue;
                        }

                        // 2. check message headers for changes
                        String subject = msg.Subject;
                        Log.d("sync", "2. Checking message " + subject);
                        if (string.IsNullOrEmpty(msg.Subject))
                        {
                            Log.d("sync", "Subject is empty - not a valid item. continue");
                            continue;
                        }

                        // 5. fetch local cache entry
                        sync.CacheEntry = cache.getEntryFromRemoteId(subject);

                        if (sync.CacheEntry == null)
                        {
                            Log.i("sync", "6. found no local entry => save");
                            status.incrementLocalNew();
                            handler.createLocalItemFromServer(sync);
                            if (sync.CacheEntry == null)
                            {
                                Log.w("sync", "createLocalItemFromServer returned a null object! See Logfile for parsing errors");
                            }

                        }
                        else
                        {
                            Log.d("sync", "7. compare data to figure out what happened");
                            if (LocalCacheProvider.isSame(sync.CacheEntry, msg))
                            {
                                Log.d("sync", "7.a/d cur=localdb");
                                if (handler.hasLocalItem(sync))
                                {
                                    Log.d("sync", "7.a check for local changes");
                                    if (handler.hasLocalChanges(sync))
                                    {
                                        Log.i("sync", "local changes found => updating ServerItem from Local");
                                        status.incrementRemoteChanged();
                                        handler.updateServerItemFromLocal(imapFolder, sync);
                                    }
                                }
                                else
                                {
                                    Log.i("sync", "7.d entry missing => delete on server");
                                    status.incrementRemoteDeleted();
                                    handler.deleteServerItem(sync);
                                }
                            }
                            else
                            {
                                Log.d("sync", "7.b/c check for local changes and \"resolve\" the conflict");
                                if (handler.hasLocalChanges(sync))
                                {
                                    Log.i("sync", "local changes found: conflicting");
                                    status.incrementConflicted();
                                    if (sync.LocalItem != null)
                                    {
                                        sync.LocalItemText = handler.GetItemText(sync);
                                    }
                                    else
                                    {
                                        sync.LocalItemText = "<deleted>";
                                    }
                                    sync.RemoteItemText = "remote changed";

                                    conflictList.Add(sync);
                                }
                                else
                                {
                                    Log.i("sync", "no local changes found => updating local item from server");
                                    status.incrementLocalChanged();
                                    handler.updateLocalItemFromServer(sync);
                                }
                            }
                        }
                    }
                    catch (SyncException ex)
                    {
                        Log.e("sync", ex.ToString());
                        status.incrementErrors(ex);
                    }
                    finally
                    {
                        cache.Save();
                    }

                    if (sync.CacheEntry != null && sync.CacheEntry.RowState != System.Data.DataRowState.Detached)
                    {
                        Log.d("sync", "8. remember message as processed (item id=" + sync.CacheEntry.localId + ")");
                        processedEntries[sync.CacheEntry.localId] = true;
                    }
                }

                // 9. for all unprocessed local items
                // 9.a upload/delete
                Log.d("sync", "9. process unprocessed local items");

                var items = handler.getAllLocalItemIDs().ToList();
                int localItemsCount = items.Count();
                int currentLocalItemNo = 1;
                foreach (var localId in items)
                {
                    if (IsStopping) return;

                    SyncContext sync = new SyncContext();
                    try
                    {
                        Log.d("sync", "9. processing #" + localId);

                        StatusHandler.writeStatus(String.Format("Processing local item {0}/{1}", currentLocalItemNo++, localItemsCount));

                        if (processedEntries.ContainsKey(localId))
                        {
                            Log.d("sync", "9.a already processed from server: skipping");
                            continue;
                        }

                        sync.CacheEntry = cache.getEntryFromLocalId(localId);
                        if (sync.CacheEntry != null)
                        {
                            Log.i("sync", "9.b found in local cache => deleting locally");
                            status.incrementLocalDeleted();
                            handler.deleteLocalItem(sync);
                        }
                        else
                        {
                            Log.i("sync", "9.c not found in local cache => creating on server");
                            status.incrementRemoteNew();
                            handler.createServerItemFromLocal(imapFolder, sync, localId);
                        }
                    }
                    catch (SyncException ex)
                    {
                        Log.e("sync", ex.ToString());
                        status.incrementErrors(ex);
                    }
                    finally
                    {
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
                cache.Save();
            }
        }
    
        private void ShowErrorDialog(Exception ex)
        {
            Helper.HandleError("Fatal error during sync", ex);
        }
    }
}