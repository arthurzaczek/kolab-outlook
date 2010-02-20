/*
 * Copyright 2010 Arthur Zaczek <arthur@dasz.at>, dasz.at OG; All rights reserved.
 * Copyright 2010 David Schmitt <david@dasz.at>, dasz.at OG; All rights reserved.
 *
 *  This file is part of Kolab Sync for Outlook.

 *  Kolab Sync for Outlook is free software: you can redistribute it
 *  and/or modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation, either version 3 of
 *  the License, or (at your option) any later version.
using OutlookKolab.Kolab.Provider;

 *  Kolab Sync for Outlook is distributed in the hope that it will be
 *  useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 *  of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  General Public License for more details.

 *  You should have received a copy of the GNU General Public License
 *  along with Kolab Sync for Outlook.  If not, see <http://www.gnu.org/licenses/>.
 */


namespace OutlookKolab.Kolab.Settings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using Outlook = Microsoft.Office.Interop.Outlook;
    
    public partial class DlgSettings : Form
    {
        Outlook.Application app;
        DSSettings dsSettings;
        DSSettings.SettingsRow settings;

        public static void Show(Outlook.Application app)
        {
            try
            {
                DlgSettings dlg = new DlgSettings(app);
                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private DlgSettings(Outlook.Application app)
        {
            this.app = app;
            InitializeComponent();
            dsSettings = DSSettings.Load();
            settings = dsSettings.Settings[0];

            BindTo();
        }

        private void BindTo()
        {
            if (!string.IsNullOrEmpty(settings.ContactsIMAPFolder) && !string.IsNullOrEmpty(settings.ContactsIMAPStore))
                txtContactsIMAPFolder.Text = app.Session.GetFolderFromID(settings.ContactsIMAPFolder, settings.ContactsIMAPStore).FullFolderPath;
            else
                txtContactsIMAPFolder.Text = "";
            if (!string.IsNullOrEmpty(settings.ContactsOutlookFolder) && !string.IsNullOrEmpty(settings.ContactsOutlookStore))
                txtContactsOutlookFolder.Text = app.Session.GetFolderFromID(settings.ContactsOutlookFolder, settings.ContactsOutlookStore).FullFolderPath;
            else
                txtContactsOutlookFolder.Text = "";

            if (!string.IsNullOrEmpty(settings.CalendarIMAPFolder) && !string.IsNullOrEmpty(settings.CalendarIMAPStore))
                txtCalendarIMAPFolder.Text = app.Session.GetFolderFromID(settings.CalendarIMAPFolder, settings.CalendarIMAPStore).FullFolderPath;
            else 
                txtCalendarIMAPFolder.Text = "";
            if (!string.IsNullOrEmpty(settings.CalendarOutlookFolder) && !string.IsNullOrEmpty(settings.CalendarOutlookStore))
                txtCalendarOutlookFolder.Text = app.Session.GetFolderFromID(settings.CalendarOutlookFolder, settings.CalendarOutlookStore).FullFolderPath;
            else 
                txtCalendarOutlookFolder.Text = "";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                dsSettings.Save();
                this.Close();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelectContactsFolder_Click(object sender, EventArgs e)
        {
            try
            {
                var fld = app.Session.PickFolder();
                settings.ContactsIMAPFolder = fld.EntryID;
                settings.ContactsIMAPStore = fld.StoreID;
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnSelectCalendarFolder_Click(object sender, EventArgs e)
        {
            try
            {
                var fld = app.Session.PickFolder();
                settings.CalendarIMAPFolder = fld.EntryID;
                settings.CalendarIMAPStore = fld.StoreID;
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnSelectOutlookContactsFolder_Click(object sender, EventArgs e)
        {
            try
            {
                var fld = app.Session.PickFolder();
                settings.ContactsOutlookFolder = fld.EntryID;
                settings.ContactsOutlookStore = fld.StoreID;
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnSelectOutlookCalendarFolder_Click(object sender, EventArgs e)
        {
            try
            {
                var fld = app.Session.PickFolder();
                settings.CalendarOutlookFolder = fld.EntryID;
                settings.CalendarOutlookStore = fld.StoreID;
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnClearContactsFolder_Click(object sender, EventArgs e)
        {
            try
            {
                settings.ContactsIMAPFolder = "";
                settings.ContactsIMAPStore = "";
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnClearOutlookContactsFolder_Click(object sender, EventArgs e)
        {
            try
            {
                settings.ContactsOutlookFolder = "";
                settings.ContactsOutlookStore = "";
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnClearCalendarFolder_Click(object sender, EventArgs e)
        {
            try
            {
                settings.CalendarIMAPFolder = "";
                settings.CalendarIMAPStore = "";
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnClearOutlookCalendarFolder_Click(object sender, EventArgs e)
        {
            try
            {
                settings.CalendarOutlookFolder = "";
                settings.CalendarOutlookStore = "";
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnDeleteCache_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure that you want to delete your Contacts and Calendar Cache?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    LocalCacheProvider.Delete(LocalCacheProviderType.Contacts);
                    LocalCacheProvider.Delete(LocalCacheProviderType.Calendar);
                }
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnDeleteContactsCache_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure that you want to delete your Contacts Cache?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    LocalCacheProvider.Delete(LocalCacheProviderType.Contacts);
                }
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnDeleteCalendarCache_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure that you want to delete your Calendar Cache?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    LocalCacheProvider.Delete(LocalCacheProviderType.Calendar);
                }
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }
    }
}
