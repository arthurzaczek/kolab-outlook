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
    using System.IO;
    using System.Linq;
    using System.Xml;

    using OutlookKolab.Kolab.Provider;
    using OutlookKolab.Kolab.Settings;
    using Outlook = Microsoft.Office.Interop.Outlook;

    public abstract class AbstractSyncHandler : ISyncHandler
    {
        protected AbstractSyncHandler(DSSettings settings, DSStatus dsStatus, Outlook.Application app)
        {
            this.app = app;
            status = dsStatus.StatusEntry.AddStatusEntryRow(DateTime.Now, "", 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.settings = settings;
        }

        protected DSStatus.StatusEntryRow status;
        protected DSSettings settings;
        protected Outlook.Application app;

        private Outlook.Folder fld = null;

       
        public abstract IEnumerable<string> getAllLocalItemIDs();
        public abstract LocalCacheProvider getLocalCacheProvider();

        public abstract String GetIMAPFolderName();
        public abstract String GetIMAPStoreID();
        public abstract String GetOutlookFolderName();
        public abstract String GetOutlookStoreID();

        public abstract string GetItemText(SyncContext sync);

        public abstract bool hasLocalChanges(SyncContext sync);
        public abstract bool hasLocalItem(SyncContext sync);

        protected abstract String getMimeType();
        
        protected abstract String writeXml(SyncContext sync);
        protected abstract String getMessageBodyText(SyncContext sync);

        protected abstract void updateLocalItemFromServer(SyncContext sync, string xml);
        protected abstract string updateServerItemFromLocal(SyncContext sync, string xml);

        protected abstract void deleteLocalItem(string localId);

        protected virtual Outlook.Folder Folder
        {
            get
            {
                if (fld == null)
                {
                    fld = (Outlook.Folder)app.Session.GetFolderFromID(GetOutlookFolderName(), GetOutlookStoreID());
                }
                return fld;
            }
        }


        public DSStatus.StatusEntryRow getStatus()
        {
            return status;
        }

        private void updateCacheEntryFromMessage(SyncContext sync)
        {
            sync.CacheEntry.remoteChangedDate = sync.Message.LastModificationTime;
            sync.CacheEntry.remoteId = sync.Message.Subject;
            sync.CacheEntry.remoteSize = sync.Message.Size;
        }

        public void createLocalItemFromServer(SyncContext sync)
        {
            Log.d("sync", "Downloading item ...");
            var xml = extractXml(sync.Message);
            updateLocalItemFromServer(sync, xml);
            updateCacheEntryFromMessage(sync);
        }

        public void updateLocalItemFromServer(SyncContext sync)
        {
            if (hasLocalItem(sync))
            {
                Log.i("sync", "Updating without conflict check: " + sync.CacheEntry.localId);
                string xml = extractXml(sync.Message);
                updateLocalItemFromServer(sync, xml);
                updateCacheEntryFromMessage(sync);
            }
        }

        public void createServerItemFromLocal(Outlook.Folder targetFolder, SyncContext sync, string localId)
        {
            Log.i("sync", "Uploading: #" + localId);

            // initialize cache entry with values that should go
            // into the new server item
            DSLocalCache.CacheEntryRow entry = getLocalCacheProvider().createEntry();
            sync.CacheEntry = entry;

            entry.localId = localId;
            String xml = writeXml(sync);
            sync.Message = wrapXmlInMessage(targetFolder, sync, xml);

            updateCacheEntryFromMessage(sync);
        }

        public void updateServerItemFromLocal(Outlook.Folder targetFolder, SyncContext sync)
        {
            Log.i("sync", "Update item on Server: #" + sync.CacheEntry.localId);

            string doc = extractXml(sync.Message);
            // Update
            string xml = updateServerItemFromLocal(sync, doc);

            // Create & Upload new Message  
            // IMAP needs a new Message uploaded
            var msgToDelete = sync.Message;
            sync.Message = wrapXmlInMessage(targetFolder, sync, xml);
            DeleteIMAPMessage(msgToDelete);
            updateCacheEntryFromMessage(sync);
        }

        public void deleteLocalItem(SyncContext sync)
        {
            Log.i("sync", "Deleting locally: " + sync.CacheEntry.localHash);
            deleteLocalItem(sync.CacheEntry.localId);
            getLocalCacheProvider().deleteEntry(sync.CacheEntry);
        }

        public void deleteServerItem(SyncContext sync)
        {
            Log.i("sync", "Deleting from server: " + sync.Message.Subject);
            DeleteIMAPMessage(sync.Message);
            getLocalCacheProvider().deleteEntry(sync.CacheEntry);
        }

        private static void DeleteIMAPMessage(Outlook.MailItem message)
        {
            message.Delete();
        }

        private static void CleanOutlookTempFolder()
        {
            //try
            //{
            //    var hkcu = Microsoft.Win32.Registry.CurrentUser;
            //    var key = hkcu.OpenSubKey(@"Software\Microsoft\Office\11.0\Outlook\Security");
            //    if(key == null)
            //    {
            //        key = hkcu.OpenSubKey(@"Software\Microsoft\Office\12.0\Outlook\Security");                    
            //    }
            //    if (key != null)
            //    {
            //        var path = key.GetValue("OutlookSecureTempFolder", string.Empty) as string;
            //        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            //        {
            //            foreach (var f in Directory.GetFiles(path, "kolab*.xml"))
            //            {
            //                try
            //                {
            //                    File.Delete(f);
            //                }
            //                catch
            //                {
            //                    // realy dont care
            //                }
            //            }
            //        }
            //    }
            //}
            //catch(Exception ex)
            //{
            //    // I don't care
            //    Log.w("outlook", ex.ToString());
            //}
        }

        private string extractXml(Outlook.MailItem message)
        {
            CleanOutlookTempFolder();
            string result = null;
            Outlook.Attachment a = message.Attachments.Cast<Outlook.Attachment>().FirstOrDefault();
            if (a != null)
            {
                var tmp = Path.GetTempFileName();
                try
                {
                    a.SaveAsFile(tmp);
                    using (var f = File.OpenText(tmp))
                    {
                        result = f.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    throw new SyncException(message.Subject, "Unable to save attachment", ex);
                }
                finally
                {
                    try { File.Delete(tmp); }
                    catch { }
                }
            }
            else
            {
                throw new SyncException(message.Subject, "Message " + message.Subject + " has not attachment");
            }
            return result;
        }

        private Outlook.MailItem wrapXmlInMessage(Outlook.Folder targetFolder, SyncContext sync, String xml)
        {
            Outlook.MailItem result = (Outlook.MailItem)targetFolder.Items.Add(Outlook.OlItemType.olMailItem);
            result.Subject = sync.CacheEntry.remoteId;
            result.Body = getMessageBodyText(sync);
            var tmpfilename = Path.GetTempFileName();
            var filename = Path.Combine(Path.GetDirectoryName(tmpfilename), Path.GetFileNameWithoutExtension(tmpfilename)) + ".xml";
            using (var f = File.CreateText(filename))
            {
                f.Write(xml);
            }
            var a = result.Attachments.Add(filename, Outlook.OlAttachmentType.olByValue, 1, "kolab.xml");
            a.PropertyAccessor.SetProperty("http://schemas.microsoft.com/mapi/proptag/0x370E001F", getMimeType());

            result.UnRead = false;

            // TODO: das geht nicht
            //result.PropertyAccessor.SetProperty("http://schemas.microsoft.com/mapi/proptag/0x00390040", now);
            //result.PropertyAccessor.SetProperty("http://schemas.microsoft.com/mapi/proptag/0x0E060040", now);

            result.Save();
            result.Move(targetFolder);
            targetFolder.Items.ResetColumns();

            File.Delete(filename);
            return result;
        }
    }
}