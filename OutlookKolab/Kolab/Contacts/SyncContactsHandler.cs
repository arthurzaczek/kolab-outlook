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

namespace OutlookKolab.Kolab.Constacts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;

    using OutlookKolab.Kolab.Provider;
    using OutlookKolab.Kolab.Settings;
    using OutlookKolab.Kolab.Sync;
    using Outlook = Microsoft.Office.Interop.Outlook;

    /// <summary>
    /// Contacts sync handler
    /// </summary>
    public class SyncContactsHandler : AbstractSyncHandler
    {
        /// <summary>
        /// Local cache provider
        /// </summary>
        LocalCacheProvider cache = null;

        /// <summary>
        /// Creates a new contacts sync handler
        /// </summary>
        /// <param name="settings">current settings</param>
        /// <param name="dsStatus">current row</param>
        /// <param name="app">Outlook Application Object</param>
        public SyncContactsHandler(DSSettings settings, DSStatus dsStatus, Outlook.Application app)
            : base(settings, dsStatus, app)
        {
            cache = new LocalCacheProvider(LocalCacheProviderType.Contacts);
            status.task = "Contacts";
        }

        /// <summary>
        /// Returns all Entry IDs of all local items
        /// </summary>
        /// <returns>List of Entry IDs</returns>
        public override IEnumerable<string> getAllLocalItemIDs()
        {
            return Folder.Items.OfType<Outlook.ContactItem>().Select(i => i.EntryID);
        }

        /// <summary>
        /// Current handlers IMAP Folder Entry ID = Remote Items
        /// </summary>
        /// <returns>Entry ID</returns>
        public override string GetIMAPFolderName()
        {
            return settings.Settings[0].ContactsIMAPFolder;
        }
        /// <summary>
        /// Current handlers IMAP Folder Store ID = Remote Items
        /// </summary>
        /// <returns>Store ID</returns>
        public override string GetIMAPStoreID()
        {
            return settings.Settings[0].ContactsIMAPStore;
        }

        /// <summary>
        /// Current handlers local Folder Entry ID = Local Items
        /// </summary>
        /// <returns>Entry ID</returns>
        public override string GetOutlookFolderName()
        {
            return settings.Settings[0].ContactsOutlookFolder;
        }
        /// <summary>
        /// Current handlers local Folder Store ID = Local Items
        /// </summary>
        /// <returns>Store ID</returns>
        public override string GetOutlookStoreID()
        {
            return settings.Settings[0].ContactsOutlookStore;
        }

        /// <summary>
        /// Returns the local cache provider of the current handler
        /// </summary>
        /// <returns>LocalCacheProvider</returns>
        public override LocalCacheProvider getLocalCacheProvider()
        {
            return cache;
        }

        /// <summary>
        /// Retreives a local contact item. SyncContext will be updated
        /// </summary>
        /// <param name="sync">current sync context.</param>
        /// <returns>Outlook.ContactItem or null if not found or item was deleted or moved</returns>
        private Outlook.ContactItem getLocalItem(SyncContext sync)
        {
            if (sync.LocalItem != null) return (Outlook.ContactItem)sync.LocalItem;
            Outlook.ContactItem result = null;
            try
            {
                result = app.Session.GetItemFromID(sync.CacheEntry.localId, Folder.StoreID) as Outlook.ContactItem;
                var fld = (Outlook.Folder)result.Parent;
                if (fld.FolderPath != Folder.FolderPath)
                {
                    // Has been deleted or moved
                    return null;
                }
            }
            catch // TODO: 
            {
                result = null;
            }
            sync.LocalItem = result;
            return result;
        }

        /// <summary>
        /// Computes the local hash of the given Outlook ContactItem
        /// </summary>
        /// <param name="item">Outlook ContactItem</param>
        /// <returns>Hash as string</returns>
        private String getLocalHash(Outlook.ContactItem item)
        {
            List<String> contents = new List<String>();
            contents.Add(item.FullName == null ? "no name" : item.FullName);

            contents.Add(item.PrimaryTelephoneNumber);
            contents.Add(item.BusinessTelephoneNumber);
            contents.Add(item.Business2TelephoneNumber);
            contents.Add(item.BusinessFaxNumber);
            contents.Add(item.CompanyMainTelephoneNumber);
            contents.Add(item.AssistantTelephoneNumber);
            contents.Add(item.CallbackTelephoneNumber);
            contents.Add(item.OtherTelephoneNumber);
            contents.Add(item.HomeTelephoneNumber);
            contents.Add(item.Home2TelephoneNumber);
            contents.Add(item.HomeFaxNumber);
            contents.Add(item.MobileTelephoneNumber);
            contents.Add(item.ISDNNumber);
            contents.Add(item.PagerNumber);
            contents.Add(item.RadioTelephoneNumber);
            contents.Add(item.TelexNumber);
            contents.Add(item.TTYTDDTelephoneNumber);
            contents.Add(item.CarTelephoneNumber);

            contents.Add(item.HomeAddress);
            contents.Add(item.BusinessAddress);
            contents.Add(item.OtherAddress);

            contents.Add(item.Email1Address);
            contents.Add(item.Email2Address);
            contents.Add(item.Email3Address);

            return String.Join("|", contents.ToArray());
        }

        /// <summary>
        /// checks for local changes
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>true if the local item changes since last sync</returns>
        public override bool hasLocalChanges(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Checking for local changes: #" + sync.CacheEntry.localId);
            var contact = getLocalItem(sync);
            String entryHash = sync.CacheEntry.localHash;
            String contactHash = contact != null ? getLocalHash(contact) : "";
            return entryHash != contactHash;
        }

        /// <summary>
        /// checks if the local item exits
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>true if the local item exits</returns>
        public override bool hasLocalItem(SyncContext sync)
        {
            return getLocalItem(sync) != null;
        }

        /// <summary>
        /// update or create the Kolab XML for a server item from a given local item and server xml
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <param name="xml">actual Kolab XML</param>
        /// <returns>new/updated Kolab XML</returns>
        protected override string updateServerItemFromLocal(SyncContext sync, string xml)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            var source = getLocalItem(sync);
            sync.CacheEntry.localHash = getLocalHash(source);
            sync.CacheEntry.remoteChangedDate = DateTime.Now.ToUniversalTime();

            var contact = Xml.XmlHelper.ParseContact(xml);
            sync.CacheEntry.remoteId = contact.uid;
            return writeXml(source, contact, sync.CacheEntry.remoteChangedDate);
        }

        /// <summary>
        /// Returns the Kolab Mime Type
        /// </summary>
        /// <returns>Mime Type - application/x-vnd.kolab.contact</returns>
        protected override string getMimeType()
        {
            return "application/x-vnd.kolab.contact";
        }

        /// <summary>
        /// Deletes a local item
        /// </summary>
        /// <param name="localId">Entry ID</param>
        protected override void deleteLocalItem(string localId)
        {
            var e = app.Session.GetItemFromID(localId, Folder.StoreID) as Outlook.ContactItem;
            if (e != null) e.Delete();
        }

        /// <summary>
        /// Update or create a local item from a given server item
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <param name="xml">Kolab XML representing the server item</param>
        protected override void updateLocalItemFromServer(SyncContext sync, string xml)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            // Parse item from given xml
            Xml.contact contact = null;
            try
            {
                contact = Xml.XmlHelper.ParseContact(xml);
            }
            catch (Exception ex)
            {
                // Unable to parse -> abort
                throw new SyncException(GetItemText(sync), "Unable to parse XML Document", ex);
            }

            // Get or add local item
            var person = (Outlook.ContactItem)sync.LocalItem;
            if (person == null)
            {
                person = (Outlook.ContactItem)Folder.Items.Add(Outlook.OlItemType.olContactItem);
            }

            try
            {
                // Basic properties
                // TODO: Add more
                if (contact.name != null) person.FullName = contact.name.fullname;

                // Phone Contacts
                if (contact.phone != null)
                {
                    foreach (var phone in contact.phone)
                    {
                        // Save based on type
                        switch (phone.type)
                        {
                            case "primary":
                                person.PrimaryTelephoneNumber = phone.number;
                                break;
                            case "business1":
                                person.BusinessTelephoneNumber = phone.number;
                                break;
                            case "business2":
                                person.Business2TelephoneNumber = phone.number;
                                break;
                            case "businessfax":
                                person.BusinessFaxNumber = phone.number;
                                break;
                            case "company":
                                person.CompanyMainTelephoneNumber = phone.number;
                                break;
                            case "assistant":
                                person.AssistantTelephoneNumber = phone.number;
                                break;
                            case "callback":
                                person.CallbackTelephoneNumber = phone.number;
                                break;
                            case "other":
                                person.OtherTelephoneNumber = phone.number;
                                break;
                            case "home1":
                                person.HomeTelephoneNumber = phone.number;
                                break;
                            case "home2":
                                person.Home2TelephoneNumber = phone.number;
                                break;
                            case "homefax":
                                person.HomeFaxNumber = phone.number;
                                break;
                            case "mobile":
                                person.MobileTelephoneNumber = phone.number;
                                break;
                            case "isdn":
                                person.ISDNNumber = phone.number;
                                break;
                            case "pager":
                                person.PagerNumber = phone.number;
                                break;
                            case "radio":
                                person.RadioTelephoneNumber = phone.number;
                                break;
                            case "telex":
                                person.TelexNumber = phone.number;
                                break;
                            case "ttytdd":
                                person.TTYTDDTelephoneNumber = phone.number;
                                break;
                            case "car":
                                person.CarTelephoneNumber = phone.number;
                                break;
                            // No default: dismiss
                        }
                    }
                }

                // Postal address
                if (contact.address != null)
                {
                    foreach (var adr in contact.address)
                    {
                        // Save based on type
                        switch (adr.type)
                        {
                            case "home":
                                person.HomeAddressCity = adr.locality;
                                person.HomeAddressCountry = adr.country;
                                person.HomeAddressState = adr.region;
                                person.HomeAddressPostalCode = adr.postalcode;
                                person.HomeAddressStreet = adr.street;
                                break;
                            case "business":
                                person.BusinessAddressCity = adr.locality;
                                person.BusinessAddressCountry = adr.country;
                                person.BusinessAddressState = adr.region;
                                person.BusinessAddressPostalCode = adr.postalcode;
                                person.BusinessAddressStreet = adr.street;
                                break;
                            case "other":
                                person.OtherAddressCity = adr.locality;
                                person.OtherAddressCountry = adr.country;
                                person.OtherAddressState = adr.region;
                                person.OtherAddressPostalCode = adr.postalcode;
                                person.OtherAddressStreet = adr.street;
                                break;
                            // No default: dismiss
                        }
                    }
                }

                // Email
                if (contact.email != null)
                {
                    int counter = 1;
                    foreach (var email in contact.email)
                    {
                        // Save based on type
                        switch (counter)
                        {
                            case 1:
                                person.Email1Address = email.smtpaddress;
                                person.Email1DisplayName = email.displayname;
                                break;
                            case 2:
                                person.Email2Address = email.smtpaddress;
                                person.Email2DisplayName = email.displayname;
                                break;
                            case 3:
                                person.Email3Address = email.smtpaddress;
                                person.Email3DisplayName = email.displayname;
                                break;
                            // No default: dismiss
                        }

                        counter++;
                    }
                }
            }
            catch (COMException ex)
            {
                // Unable to set properties -> abort
                throw new SyncException(GetItemText(sync), "Unable to set basic ContactItem options", ex);
            }

            try
            {
                person.Save();
            }
            catch (COMException ex)
            {
                // Unable to sace -> abort
                throw new SyncException(GetItemText(sync), "Unable to save ContactItem", ex);
            }

            // Create local cache entry if a new item was created
            if (sync.CacheEntry == null)
            {
                sync.CacheEntry = getLocalCacheProvider().createEntry();
            }
            // Upate local cache entry
            sync.CacheEntry.localId = person.EntryID;
            sync.CacheEntry.localHash = getLocalHash(person);
        }

        /// <summary>
        /// Create Application and Type specific id.
        /// ko == Kolab Outlook, ct == contact
        /// </summary>
        /// <returns>new UID</returns>
        private String getNewUid()
        {
            // Create Application and Type specific id
            // ko == Kolab Outlook
            return "ko-ct-" + Guid.NewGuid();
        }

        /// <summary>
        /// Creates a Kolab XML string. This method also must update the local cache entry.
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>xml string</returns>
        protected override string writeXml(SyncContext sync)
        {
            var item = getLocalItem(sync);
            sync.CacheEntry.localHash = getLocalHash(item);
            sync.CacheEntry.remoteChangedDate = DateTime.Now.ToUniversalTime();
            sync.CacheEntry.remoteId = getNewUid();
            return writeXml(item, new OutlookKolab.Kolab.Xml.contact() { uid = sync.CacheEntry.remoteId }, sync.CacheEntry.remoteChangedDate);
        }

        /// <summary>
        /// Creates a Kolab XML string.
        /// </summary>
        /// <param name="source">Outlook Item</param>
        /// <param name="cal">destination calendar XML Object</param>
        /// <param name="lastmodificationdate">last modification date</param>
        /// <returns>xml string</returns>
        private string writeXml(Microsoft.Office.Interop.Outlook.ContactItem source, OutlookKolab.Kolab.Xml.contact contact, DateTime lastmodificationdate)
        {
            // Basic properties
            contact.lastmodificationdate = lastmodificationdate;
            if (contact.name == null) contact.name = new OutlookKolab.Kolab.Xml.contactName();
            contact.name.fullname = source.FullName;

            // Phone contact methods
            var phones = new List<OutlookKolab.Kolab.Xml.contactPhone>();
            if (!string.IsNullOrEmpty(source.PrimaryTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "primary", number = source.PrimaryTelephoneNumber });
            if (!string.IsNullOrEmpty(source.BusinessTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "business1", number = source.BusinessTelephoneNumber });
            if (!string.IsNullOrEmpty(source.Business2TelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "business2", number = source.Business2TelephoneNumber });
            if (!string.IsNullOrEmpty(source.BusinessFaxNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "businessfax", number = source.BusinessFaxNumber });
            if (!string.IsNullOrEmpty(source.CompanyMainTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "company", number = source.CompanyMainTelephoneNumber });
            if (!string.IsNullOrEmpty(source.AssistantTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "assistant", number = source.AssistantTelephoneNumber });
            if (!string.IsNullOrEmpty(source.CallbackTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "callback", number = source.CallbackTelephoneNumber });
            if (!string.IsNullOrEmpty(source.OtherTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "other", number = source.OtherTelephoneNumber });
            if (!string.IsNullOrEmpty(source.HomeTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "home1", number = source.HomeTelephoneNumber });
            if (!string.IsNullOrEmpty(source.Home2TelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "home2", number = source.Home2TelephoneNumber });
            if (!string.IsNullOrEmpty(source.HomeFaxNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "homefax", number = source.HomeFaxNumber });
            if (!string.IsNullOrEmpty(source.MobileTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "mobile", number = source.MobileTelephoneNumber });
            if (!string.IsNullOrEmpty(source.ISDNNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "isdn", number = source.ISDNNumber });
            if (!string.IsNullOrEmpty(source.PagerNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "pager", number = source.PagerNumber });
            if (!string.IsNullOrEmpty(source.RadioTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "radio", number = source.RadioTelephoneNumber });
            if (!string.IsNullOrEmpty(source.TelexNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "telex", number = source.TelexNumber });
            if (!string.IsNullOrEmpty(source.TTYTDDTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "ttytdd", number = source.TTYTDDTelephoneNumber });
            if (!string.IsNullOrEmpty(source.CarTelephoneNumber)) phones.Add(new OutlookKolab.Kolab.Xml.contactPhone() { type = "car", number = source.CarTelephoneNumber });
            contact.phone = phones.ToArray();

            // Postal address contact methods
            var adrs = new List<OutlookKolab.Kolab.Xml.contactAddress>();
            if (!string.IsNullOrEmpty(source.HomeAddress)) adrs.Add(new OutlookKolab.Kolab.Xml.contactAddress() { type = "home", country = source.HomeAddressCountry, locality = source.HomeAddressCity, region = source.HomeAddressState, postalcode = source.HomeAddressPostalCode, street = source.HomeAddressStreet });
            if (!string.IsNullOrEmpty(source.BusinessAddress)) adrs.Add(new OutlookKolab.Kolab.Xml.contactAddress() { type = "business", country = source.BusinessAddressCountry, locality = source.BusinessAddressCity, region = source.BusinessAddressState, postalcode = source.BusinessAddressPostalCode, street = source.BusinessAddressStreet });
            if (!string.IsNullOrEmpty(source.OtherAddress)) adrs.Add(new OutlookKolab.Kolab.Xml.contactAddress() { type = "other", country = source.OtherAddressCountry, locality = source.OtherAddressCity, region = source.OtherAddressState, postalcode = source.OtherAddressPostalCode, street = source.OtherAddressStreet });
            contact.address = adrs.ToArray();

            // EMail contact methods
            var emails = new List<OutlookKolab.Kolab.Xml.contactEmail>();
            if (!string.IsNullOrEmpty(source.Email1Address)) emails.Add(new OutlookKolab.Kolab.Xml.contactEmail() { displayname = source.Email1DisplayName, smtpaddress = source.Email1Address });
            if (!string.IsNullOrEmpty(source.Email2Address)) emails.Add(new OutlookKolab.Kolab.Xml.contactEmail() { displayname = source.Email2DisplayName, smtpaddress = source.Email2Address });
            if (!string.IsNullOrEmpty(source.Email3Address)) emails.Add(new OutlookKolab.Kolab.Xml.contactEmail() { displayname = source.Email3DisplayName, smtpaddress = source.Email3Address });
            contact.email = emails.ToArray();

            return Xml.XmlHelper.ToString(contact);
        }

        /// <summary>
        /// Creates a MailMessage body text
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>MailMessage body text</returns>
        public override string getMessageBodyText(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            var contact = getLocalItem(sync);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(contact.FullName);
            sb.AppendLine("----- Contact Methods -----");
            sb.AppendLine("primary: " + contact.PrimaryTelephoneNumber);
            sb.AppendLine("business1: " + contact.BusinessTelephoneNumber);
            sb.AppendLine("business2: " + contact.Business2TelephoneNumber);
            sb.AppendLine("businessfax: " + contact.BusinessFaxNumber);
            sb.AppendLine("company: " + contact.CompanyMainTelephoneNumber);
            sb.AppendLine("assistant: " + contact.AssistantTelephoneNumber);
            sb.AppendLine("callback: " + contact.CallbackTelephoneNumber);
            sb.AppendLine("other: " + contact.OtherTelephoneNumber);
            sb.AppendLine("home1: " + contact.HomeTelephoneNumber);
            sb.AppendLine("home2: " + contact.Home2TelephoneNumber);
            sb.AppendLine("homefax: " + contact.HomeFaxNumber);
            sb.AppendLine("mobile: " + contact.MobileTelephoneNumber);
            sb.AppendLine("isdn: " + contact.ISDNNumber);
            sb.AppendLine("pager: " + contact.PagerNumber);
            sb.AppendLine("radio: " + contact.RadioTelephoneNumber);
            sb.AppendLine("telex: " + contact.TelexNumber);
            sb.AppendLine("ttytdd: " + contact.TTYTDDTelephoneNumber);
            sb.AppendLine("car: " + contact.CarTelephoneNumber);

            sb.AppendLine("----- Adresses -----");
            sb.AppendLine("home: " + contact.HomeAddress);
            sb.AppendLine("business: " + contact.BusinessAddress);
            sb.AppendLine("other: " + contact.OtherAddress);

            sb.AppendLine("----- Mails -----");
            sb.AppendLine("email1: " + contact.Email1Address + " (" + contact.Email1DisplayName + ")");
            sb.AppendLine("email1: " + contact.Email2Address + " (" + contact.Email2DisplayName + ")");
            sb.AppendLine("email1: " + contact.Email3Address + " (" + contact.Email3DisplayName + ")");

            return sb.ToString();
        }

        /// <summary>
        /// Short text of the current local item
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>short text</returns>
        public override string GetItemText(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            var item = sync.LocalItem as Outlook.ContactItem;
            if (item != null)
            {
                return item.FullName;
            }
            else
            {
                return sync.Message.Subject;
            }
        }
    }
}