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

    public abstract class AbstractSyncHandler
        : ISyncHandler, IDisposable
    {
        protected AbstractSyncHandler(DSSettings settings, DSStatus dsStatus, Outlook.Application app)
        {
            this.app = app;
            status = dsStatus.StatusEntry.AddStatusEntryRow(DateTime.Now, String.Empty, 0, 0, 0, 0, 0, 0, 0, 0, 0);
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
        public abstract String getMessageBodyText(SyncContext sync);

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
            sync.CacheEntry.remoteChangedDate = sync.Message.GetChangedDate();
            sync.CacheEntry.remoteId = sync.Message.Subject;
            sync.CacheEntry.remoteSize = sync.Message.Size;
        }

        public void createLocalItemFromServer(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

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

        public void createServerItemFromLocal(Outlook.Folder imapFolder, SyncContext sync, string localId)
        {
            if (imapFolder == null) { throw new ArgumentNullException("imapFolder"); }
            if (sync == null) { throw new ArgumentNullException("sync"); }
            if (String.IsNullOrEmpty(localId)) { throw new ArgumentNullException("localId"); }

            Log.i("sync", "Uploading: #" + localId);

            // initialize cache entry with values that should go
            // into the new server item
            DSLocalCache.CacheEntryRow entry = getLocalCacheProvider().createEntry();
            sync.CacheEntry = entry;

            entry.localId = localId;
            String xml = writeXml(sync);
            sync.Message = wrapXmlInMessage(imapFolder, sync, xml);

            updateCacheEntryFromMessage(sync);
        }

        public void updateServerItemFromLocal(Outlook.Folder imapFolder, SyncContext sync)
        {
            if (imapFolder == null) { throw new ArgumentNullException("imapFolder"); }
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Update item on Server: #" + sync.CacheEntry.localId);

            string doc = extractXml(sync.Message);
            // Update
            string xml = updateServerItemFromLocal(sync, doc);

            // Create & Upload new Message  
            // IMAP needs a new Message uploaded
            var msgToDelete = sync.Message;
            sync.Message = wrapXmlInMessage(imapFolder, sync, xml);
            DeleteIMAPMessage(msgToDelete);
            updateCacheEntryFromMessage(sync);
        }

        public void deleteLocalItem(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Deleting locally: " + sync.CacheEntry.localHash);
            deleteLocalItem(sync.CacheEntry.localId);
            getLocalCacheProvider().deleteEntry(sync.CacheEntry);
        }

        public void deleteServerItem(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Deleting from server: " + sync.Message.Subject);
            DeleteIMAPMessage(sync.Message);
            getLocalCacheProvider().deleteEntry(sync.CacheEntry);
        }

        private static void DeleteIMAPMessage(Outlook.MailItem message)
        {
            message.Delete();
        }

        private string extractXml(Outlook.MailItem message)
        {
            string result = null;
            Outlook.Attachment a = message.Attachments.Cast<Outlook.Attachment>().FirstOrDefault();
            if (a != null)
            {
                IntPtr ptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(a.MAPIOBJECT);
                try
                {
                    result = OutlookKolabMAPIHelper.IMAPHelper.ReadAttachment(ptr);
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.Release(ptr);
                }
            }
            else
            {
                throw new SyncException(message.Subject, "Message " + message.Subject + " has not attachment");
            }
            return result;
        }

        private Outlook.MailItem wrapXmlInMessage(Outlook.Folder imapFolder, SyncContext sync, String xml)
        {
            Outlook.MailItem result = (Outlook.MailItem)imapFolder.Items.Add(Outlook.OlItemType.olMailItem);
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

            var now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second); // Kill miliseconds - not stored on the server
            IntPtr ptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(result.MAPIOBJECT);
            try
            {
                OutlookKolabMAPIHelper.IMAPHelper.SetSentDate(ptr, now);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.Release(ptr);
            }

            result.Save();
            result.Move(imapFolder);

            result = (Outlook.MailItem)imapFolder.Items[sync.CacheEntry.remoteId];

            File.Delete(filename);
            return result;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (settings != null)
            {
                settings.Dispose();
                settings = null;
            }
        }

        #endregion
    }
}