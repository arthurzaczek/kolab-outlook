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

    /// <summary>
    /// Abstract base class for all sync handler
    /// </summary>
    public abstract class AbstractSyncHandler
        : ISyncHandler, IDisposable
    {
        /// <summary>
        /// Creates a new sync handler
        /// </summary>
        /// <param name="settings">current settings</param>
        /// <param name="dsStatus">current row</param>
        /// <param name="app">Outlook Application Object</param>
        protected AbstractSyncHandler(DSSettings settings, DSStatus dsStatus, Outlook.Application app)
        {
            this.app = app;
            status = dsStatus.StatusEntry.AddStatusEntryRow(DateTime.Now, String.Empty, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.settings = settings;
        }

        /// <summary>
        /// Current status cache
        /// </summary>
        protected DSStatus.StatusEntryRow status;
        /// <summary>
        /// Current settings cache
        /// </summary>
        protected DSSettings settings;
        /// <summary>
        /// Outlook Application Object cache
        /// </summary>
        protected Outlook.Application app;

        /// <summary>
        /// Current local items folder cache
        /// </summary>
        private Outlook.Folder fld = null;

        /// <summary>
        /// Returns all Entry IDs of all local items
        /// </summary>
        /// <returns>List of Entry IDs</returns>
        public abstract IList<string> getAllLocalItemIDs();
        /// <summary>
        /// Returns the local cache provider of the current handler
        /// </summary>
        /// <returns>LocalCacheProvider</returns>
        public abstract LocalCacheProvider getLocalCacheProvider();

        /// <summary>
        /// Current handlers IMAP Folder Entry ID = Remote Items
        /// </summary>
        /// <returns>Entry ID</returns>
        public abstract String GetIMAPFolderName();
        /// <summary>
        /// Current handlers IMAP Folder Store ID = Remote Items
        /// </summary>
        /// <returns>Store ID</returns>
        public abstract String GetIMAPStoreID();
        /// <summary>
        /// Current handlers local Folder Entry ID = Local Items
        /// </summary>
        /// <returns>Entry ID</returns>
        public abstract String GetOutlookFolderName();
        /// <summary>
        /// Current handlers local Folder Store ID = Local Items
        /// </summary>
        /// <returns>Store ID</returns>
        public abstract String GetOutlookStoreID();

        /// <summary>
        /// Short text of the current local item
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>short text</returns>
        public abstract string GetItemText(SyncContext sync);

        /// <summary>
        /// checks for local changes
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>true if the local item changes since last sync</returns>
        public abstract bool hasLocalChanges(SyncContext sync);
        /// <summary>
        /// checks if the local item exits
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>true if the local item exits</returns>
        public abstract bool hasLocalItem(SyncContext sync);

        /// <summary>
        /// Returns the Kolab Mime Type
        /// </summary>
        /// <returns>Mime Type</returns>
        protected abstract String getMimeType();

        /// <summary>
        /// Creates a Kolab XML string. This method also must update the local cache entry.
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>xml string</returns>
        protected abstract String createNewXml(SyncContext sync);
        /// <summary>
        /// Creates a MailMessage body text
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>MailMessage body text</returns>
        public abstract String getMessageBodyText(SyncContext sync);

        /// <summary>
        /// Update or create a local item from a given server item
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <param name="xml">Kolab XML representing the server item</param>
        protected abstract void updateLocalItemFromServer(SyncContext sync, string xml);
        /// <summary>
        /// update or create the Kolab XML for a server item from a given local item and server xml
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <param name="xml">actual Kolab XML</param>
        /// <returns>new/updated Kolab XML</returns>
        protected abstract string updateServerItemFromLocal(SyncContext sync, string xml);

        /// <summary>
        /// Deletes a local item
        /// </summary>
        /// <param name="localId">Entry ID</param>
        protected abstract void deleteLocalItem(string localId);

        /// <summary>
        /// Current local items folder.
        /// </summary>
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

        /// <summary>
        /// Returns the current status row
        /// </summary>
        /// <returns>DSStatus.StatusEntryRow</returns>
        public DSStatus.StatusEntryRow getStatus()
        {
            return status;
        }

        /// <summary>
        /// Update given cache entry from given MailMessage
        /// </summary>
        /// <param name="sync">current sync context</param>
        private void updateCacheEntryFromMessage(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            sync.CacheEntry.remoteChangedDate = sync.Message.GetChangedDate();
            sync.CacheEntry.remoteId = sync.Message.Subject;
            sync.CacheEntry.remoteSize = sync.Message.Size;
        }

        /// <summary>
        /// create a local item from the given server items
        /// </summary>
        /// <param name="sync">current sync context</param>
        public void createLocalItemFromServer(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.d("sync", "Downloading item ...");
            var xml = extractXml(sync.Message);
            updateLocalItemFromServer(sync, xml);
            updateCacheEntryFromMessage(sync);
        }

        /// <summary>
        /// update a local item from the given server items
        /// </summary>
        /// <param name="sync">current sync context</param>
        public void updateLocalItemFromServer(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }
            
            if (hasLocalItem(sync))
            {
                Log.d("sync", "Downloading item ...");
                string xml = extractXml(sync.Message);
                updateLocalItemFromServer(sync, xml);
                updateCacheEntryFromMessage(sync);
            }
        }

        /// <summary>
        /// Create a new server item from the given local item.
        /// This method creates also a new local cache entry.
        /// </summary>
        /// <param name="imapFolder">destination IMAP Folder</param>
        /// <param name="sync">current sync context</param>
        /// <param name="localId">Entry ID of the local item</param>
        public void createServerItemFromLocal(Outlook.Folder imapFolder, SyncContext sync, string localId)
        {
            if (imapFolder == null) { throw new ArgumentNullException("imapFolder"); }
            if (sync == null) { throw new ArgumentNullException("sync"); }
            if (String.IsNullOrEmpty(localId)) { throw new ArgumentNullException("localId"); }

            Log.i("sync", "Uploading: #" + localId);

            // initialize cache entry with values that should go
            // into the new server item
            sync.CacheEntry = getLocalCacheProvider().createEntry();

            sync.CacheEntry.localId = localId;
            String xml = createNewXml(sync);
            sync.Message = wrapXmlInMessage(imapFolder, sync, xml);

            updateCacheEntryFromMessage(sync);
        }

        /// <summary>
        /// Update a server item from the given local item.
        /// </summary>
        /// <param name="imapFolder">destination IMAP Folder</param>
        /// <param name="sync">current sync context</param>
        public void updateServerItemFromLocal(Outlook.Folder imapFolder, SyncContext sync)
        {
            if (imapFolder == null) { throw new ArgumentNullException("imapFolder"); }
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Update item on Server: #" + sync.CacheEntry.localId);

            // get Message XML
            string doc = extractXml(sync.Message);
            // Update and get local XML
            string xml = updateServerItemFromLocal(sync, doc);

            // Create & Upload new Message  
            // IMAP needs a new Message uploaded
            var msgToDelete = sync.Message;
            sync.Message = wrapXmlInMessage(imapFolder, sync, xml);
            DeleteIMAPMessage(msgToDelete);
            updateCacheEntryFromMessage(sync);
        }

        /// <summary>
        /// Deletes the given local item
        /// </summary>
        /// <param name="sync">current sync context</param>
        public void deleteLocalItem(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Deleting locally: " + sync.CacheEntry.localHash);
            deleteLocalItem(sync.CacheEntry.localId);
            getLocalCacheProvider().deleteEntry(sync.CacheEntry);
        }

        /// <summary>
        /// Deletes the given server item
        /// </summary>
        /// <param name="sync">current sync context</param>
        public void deleteServerItem(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Deleting from server: " + sync.Message.Subject);
            DeleteIMAPMessage(sync.Message);
            getLocalCacheProvider().deleteEntry(sync.CacheEntry);
        }

        /// <summary>
        /// Deletes the given IMAP MailMessage
        /// </summary>
        /// <param name="message">Outlooks MailMessage item</param>
        private static void DeleteIMAPMessage(Outlook.MailItem message)
        {
            message.Delete();
        }

        /// <summary>
        /// Exstract the Kolab XML Document from the given MailMessage
        /// </summary>
        /// <param name="message">Outlooks MailMessage item</param>
        /// <returns>Kolab XML String</returns>
        private string extractXml(Outlook.MailItem message)
        {
            string result = null;
            // Take the first attachment
            // TODO: Look for the first attachment with current Kolab MimeType
            Outlook.Attachment a = message.Attachments.Cast<Outlook.Attachment>().FirstOrDefault();
            if (a != null)
            {
                // Get an IUnknown Pointer of that attachment
                IntPtr ptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(a.MAPIOBJECT);
                try
                {
                    // Call our little C++ helper to extract the attachment in memory
                    // Calling a.SaveAs(...) would lead to a bunch of troubles
                    // If all Attachments has the same name (like on a kolab server)
                    // then Outlook is only able to store 100 (yes! 100!) Attachments
                    // Why? Because it saves the attachment in a TempFolder (see Registry)
                    // Then it opens a FileSystemWatcher (or the Win32 API equivalent)
                    // But: If that filename already exists Outlook behaves like the Windows Explorer
                    // It creates kolab (1).xml, kolab (2).xml, ..., kolab (99).xml files
                    // Outlook is'nt able to save kolab (100).xml - I dont know why, 
                    // but here is our 100 Attachments limit
                    result = OutlookKolabMAPIHelper.IMAPHelper.ReadAttachment(ptr);
                }
                finally
                {
                    // Release that COM Pointer
                    System.Runtime.InteropServices.Marshal.Release(ptr);
                }
            }
            else
            {
                // No Attachment found -> throw a SyncException an continue
                throw new SyncException(message.Subject, "Message " + message.Subject + " has not attachment");
            }
            return result;
        }

        /// <summary>
        /// Creates a new IMAP Mail item.
        /// </summary>
        /// <param name="imapFolder">destination IMAP Folder</param>
        /// <param name="sync">current sync context</param>
        /// <param name="xml">Kolab XML to store in a attachment</param>
        /// <returns>the new IMAP Mail Message</returns>
        private Outlook.MailItem wrapXmlInMessage(Outlook.Folder imapFolder, SyncContext sync, String xml)
        {
            // Create the new message
            Outlook.MailItem result = (Outlook.MailItem)imapFolder.Items.Add(Outlook.OlItemType.olMailItem);
            // Set the easy parts of the message
            result.Subject = sync.CacheEntry.remoteId;
            result.Body = getMessageBodyText(sync);

            // Save the XML File in a Temp file
            // TODO: Call the little C++ helper to store the attachment directly
            var tmpfilename = Path.GetTempFileName();
            var filename = Path.Combine(Path.GetDirectoryName(tmpfilename), Path.GetFileNameWithoutExtension(tmpfilename)) + ".xml";
            using (var f = File.CreateText(filename))
            {
                f.Write(xml);
            }
            // Create the attachment
            var a = result.Attachments.Add(filename, Outlook.OlAttachmentType.olByValue, 1, "kolab.xml");
            // Use Trick #17 to set the correct MimeType
            a.PropertyAccessor.SetProperty("http://schemas.microsoft.com/mapi/proptag/0x370E001F", getMimeType());

            // Mark as read
            result.UnRead = false;

            // Get current DateTime
            var now = DateTime.Now;
            // Kill miliseconds - not stored on the server
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second); 
            // Get a COM Pointer of the newly created MailMessage for the little C++ helper
            IntPtr ptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(result.MAPIOBJECT);
            try
            {
                // Set SentDate
                // Outlook will not allow to set the SentDate throw it's own Model
                // SentDate is read only, calling PropertyAccessor leads to an exception
                // "If you are unwilling then I have to use C++"
                // "Und bist Du nicht willig so brauch ich C++"
                OutlookKolabMAPIHelper.IMAPHelper.SetSentDate(ptr, now);
            }
            finally
            {
                // Release that COM Pointer
                System.Runtime.InteropServices.Marshal.Release(ptr);
            }

            // Save the newly created Message
            result.Save();
            // The Message is stored in the Drafts Folder - move it to the destination folder
            result.Move(imapFolder);

            // Reload the item
            // SentDate was set through MAPI directly so Outlook doesn't know about
            // Calling app.Session.GetItemFromEntryID(..) will lead to an exception
            // I think because the current session is out of sync or something
            // The error message suggest to wait a little bit
            // Fetching the Message from the Folders Items collection will bring
            // the new MailMessage immediately
            result = (Outlook.MailItem)imapFolder.Items[sync.CacheEntry.remoteId];

            // Delete temp. file
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