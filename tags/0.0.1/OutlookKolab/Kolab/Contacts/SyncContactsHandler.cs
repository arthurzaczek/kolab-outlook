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

    public class SyncContactsHandler : AbstractSyncHandler
    {
        LocalCacheProvider cache = null;

        public SyncContactsHandler(DSSettings settings, DSStatus dsStatus, Outlook.Application app)
            : base(settings, dsStatus, app)
        {
            cache = new LocalCacheProvider(LocalCacheProviderType.Contacts);
            status.task = "Contacts";
        }

        public override IEnumerable<string> getAllLocalItemIDs()
        {
            return Folder.Items.OfType<Outlook.ContactItem>().Select(i => i.EntryID);
        }

        public override string GetIMAPFolderName()
        {
            return settings.Settings[0].ContactsIMAPFolder;
        }
        public override string GetIMAPStoreID()
        {
            return settings.Settings[0].ContactsIMAPStore;
        }

        public override string GetOutlookFolderName()
        {
            return settings.Settings[0].ContactsOutlookFolder;
        }
        public override string GetOutlookStoreID()
        {
            return settings.Settings[0].ContactsOutlookStore;
        }

        public override LocalCacheProvider getLocalCacheProvider()
        {
            return cache;
        }

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

        public override bool hasLocalChanges(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Checking for local changes: #" + sync.CacheEntry.localId);
            var contact = getLocalItem(sync);
            String entryHash = sync.CacheEntry.localHash;
            String contactHash = contact != null ? getLocalHash(contact) : "";
            return entryHash != contactHash;
        }

        protected override string updateServerItemFromLocal(SyncContext sync, string xml)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            var source = getLocalItem(sync);
            sync.CacheEntry.localHash = getLocalHash(source);
            sync.CacheEntry.remoteChangedDate = DateTime.Now.ToUniversalTime();

            var contact = Xml.XmlHelper.ParseContact(xml);
            return writeXml(source, contact, sync.CacheEntry.remoteChangedDate);
        }

        protected override string getMimeType()
        {
            return "application/x-vnd.kolab.contact";
        }

        protected override void deleteLocalItem(string localId)
        {
            var e = app.Session.GetItemFromID(localId, Folder.StoreID) as Outlook.ContactItem;
            if (e != null) e.Delete();
        }

        public override bool hasLocalItem(SyncContext sync)
        {
            return getLocalItem(sync) != null;
        }

        protected override void updateLocalItemFromServer(SyncContext sync, string xml)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Xml.contact contact = null;
            try
            {
                contact = Xml.XmlHelper.ParseContact(xml);
            }
            catch (Exception ex)
            {
                throw new SyncException(GetItemText(sync), "Unable to parse XML Document", ex);
            }

            var person = (Outlook.ContactItem)sync.LocalItem;
            if (person == null)
            {
                person = (Outlook.ContactItem)Folder.Items.Add(Outlook.OlItemType.olContactItem);
            }

            try
            {
                if (contact.name != null) person.FullName = contact.name.fullname;

                if (contact.phone != null)
                {
                    foreach (var phone in contact.phone)
                    {
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
                        }
                    }
                }

                if (contact.address != null)
                {
                    foreach (var adr in contact.address)
                    {
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
                        }
                    }
                }

                if (contact.email != null)
                {
                    int counter = 1;
                    foreach (var email in contact.email)
                    {
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
                        }

                        counter++;
                    }
                }
            }
            catch (COMException ex)
            {
                throw new SyncException(GetItemText(sync), "Unable to set basic ContactItem options", ex);
            }

            try
            {
                person.Save();
            }
            catch (COMException ex)
            {
                throw new SyncException(GetItemText(sync), "Unable to save ContactItem", ex);
            }            

            if (sync.CacheEntry == null)
            {
                sync.CacheEntry = getLocalCacheProvider().createEntry();
            }
            sync.CacheEntry.localId = person.EntryID;
            sync.CacheEntry.localHash = getLocalHash(person);
        }

        private String getNewUid()
        {
            // Create Application and Type specific id
            // ko == Kolab Outlook
            return "ko-ct-" + Guid.NewGuid();
        }

        protected override string writeXml(SyncContext sync)
        {
            var item = getLocalItem(sync);
            sync.CacheEntry.localHash = getLocalHash(item);
            sync.CacheEntry.remoteChangedDate = DateTime.Now.ToUniversalTime();
            sync.CacheEntry.remoteId = getNewUid();
            return writeXml(item, new OutlookKolab.Kolab.Xml.contact(), sync.CacheEntry.remoteChangedDate);
        }

        private string writeXml(Microsoft.Office.Interop.Outlook.ContactItem source, OutlookKolab.Kolab.Xml.contact contact, DateTime lastmodificationdate)
        {
            contact.lastmodificationdate = lastmodificationdate;
            if (contact.name == null) contact.name = new OutlookKolab.Kolab.Xml.contactName();
            contact.name.fullname = source.FullName;

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

            var adrs = new List<OutlookKolab.Kolab.Xml.contactAddress>();
            if (!string.IsNullOrEmpty(source.HomeAddress)) adrs.Add(new OutlookKolab.Kolab.Xml.contactAddress() { type = "home", country = source.HomeAddressCountry, locality = source.HomeAddressCity, region = source.HomeAddressState, postalcode = source.HomeAddressPostalCode, street = source.HomeAddressStreet });
            if (!string.IsNullOrEmpty(source.BusinessAddress)) adrs.Add(new OutlookKolab.Kolab.Xml.contactAddress() { type = "business", country = source.BusinessAddressCountry, locality = source.BusinessAddressCity, region = source.BusinessAddressState, postalcode = source.BusinessAddressPostalCode, street = source.BusinessAddressStreet });
            if (!string.IsNullOrEmpty(source.OtherAddress)) adrs.Add(new OutlookKolab.Kolab.Xml.contactAddress() { type = "other", country = source.OtherAddressCountry, locality = source.OtherAddressCity, region = source.OtherAddressState, postalcode = source.OtherAddressPostalCode, street = source.OtherAddressStreet });
            contact.address = adrs.ToArray();

            var emails = new List<OutlookKolab.Kolab.Xml.contactEmail>();
            if (!string.IsNullOrEmpty(source.Email1Address)) emails.Add(new OutlookKolab.Kolab.Xml.contactEmail() { displayname = source.Email1DisplayName, smtpaddress = source.Email1Address });
            if (!string.IsNullOrEmpty(source.Email2Address)) emails.Add(new OutlookKolab.Kolab.Xml.contactEmail() { displayname = source.Email2DisplayName, smtpaddress = source.Email2Address });
            if (!string.IsNullOrEmpty(source.Email3Address)) emails.Add(new OutlookKolab.Kolab.Xml.contactEmail() { displayname = source.Email3DisplayName, smtpaddress = source.Email3Address });
            contact.email = emails.ToArray();

            return Xml.XmlHelper.ToString(contact);
        }

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