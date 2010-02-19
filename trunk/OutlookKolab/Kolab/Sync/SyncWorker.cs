using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Xml;
using System.Threading;
using OutlookKolab.Kolab.Provider;
using OutlookKolab.Kolab.Constacts;
using OutlookKolab.Kolab.Calendar;
using System.Reflection;
using System.Windows.Forms;

namespace OutlookKolab.Kolab.Sync
{
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

                if (IsStopping) return;

                handler = new SyncContactsHandler(settings, dsStatus, app);
                if (shouldProcess(handler))
                {
                    _status = handler.getStatus();
                    sync(handler);
                    hasErrors = _status.errors > 0;
                    dsStatus.Save();
                }

                if (IsStopping) return;

                handler = new SyncCalendarHandler(settings, dsStatus, app);
                if (shouldProcess(handler))
                {
                    _status = handler.getStatus();
                    sync(handler);
                    hasErrors = _status.errors > 0;
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
                var sourceFolder = (Outlook.Folder)app.Session.GetFolderFromID(handler.GetIMAPFolderName(), handler.GetIMAPStoreID());

                Dictionary<string, bool> processedEntries = new Dictionary<string, bool>();
                DSStatus.StatusEntryRow status = handler.getStatus();

                ILookup<string, string> deletedEntryIDs = null;
                try
                {
                    OutlookKolapMAPIHelper.IMAPFolderHelper mapi = new OutlookKolapMAPIHelper.IMAPFolderHelper();
                    IntPtr ptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(sourceFolder.MAPIOBJECT);
                    deletedEntryIDs = mapi.GetDeletedEntryIDs(ptr).ToLookup(i => i);
                    System.Runtime.InteropServices.Marshal.Release(ptr);
                }
                catch (Exception ex)
                {
                    Log.e("imap", ex.ToString());
                    throw;
                }


                var msgList = sourceFolder.Items.OfType<Outlook.MailItem>().ToList();
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
                                    Log.d("sync", "7.a check for local changes and upload them");
                                    if (handler.hasLocalChanges(sync))
                                    {
                                        Log.i("sync", "local changes found: updating ServerItem from Local");
                                        status.incrementRemoteChanged();
                                        handler.updateServerItemFromLocal(sourceFolder, sync);
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
                                    ShowConflictDialog();
                                }
                                else
                                {
                                    Log.i("sync", "no local changes found: updating local item from server");
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
                            Log.i("sync", "9.b found in local cache: deleting locally");
                            status.incrementLocalDeleted();
                            handler.deleteLocalItem(sync);
                        }
                        else
                        {
                            Log.i("sync", "9.c not found in local cache: creating on server");
                            status.incrementRemoteNew();
                            handler.createServerItemFromLocal(sourceFolder, sync, localId);
                        }
                    }
                    catch (SyncException ex)
                    {
                        Log.e("sync", ex.ToString());
                        status.incrementErrors(ex);
                    }
                }
            }
            finally
            {
                cache.Save();
            }
        }

        private void ShowConflictDialog()
        {
            // StatusHandler.writeStatus("sync conflict");
        }
    
        private void ShowErrorDialog(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Fatal error during sync", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}