using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Outlook = Microsoft.Office.Interop.Outlook;

namespace OutlookKolab.Kolab.Settings
{
    public partial class DlgSettings : Form
    {
        Outlook.Application app;
        DSSettings dsSettings;
        DSSettings.SettingsRow settings;

        public static void Show(Outlook.Application app)
        {
            DlgSettings dlg = new DlgSettings(app);
            dlg.ShowDialog();
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
            dsSettings.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelectContactsFolder_Click(object sender, EventArgs e)
        {
            var fld = app.Session.PickFolder();
            settings.ContactsIMAPFolder = fld.EntryID;
            settings.ContactsIMAPStore = fld.StoreID;
            BindTo();
        }

        private void btnSelectCalendarFolder_Click(object sender, EventArgs e)
        {
            var fld = app.Session.PickFolder();
            settings.CalendarIMAPFolder = fld.EntryID;
            settings.CalendarIMAPStore = fld.StoreID;
            BindTo();
        }

        private void btnSelectOutlookContactsFolder_Click(object sender, EventArgs e)
        {
            var fld = app.Session.PickFolder();
            settings.ContactsOutlookFolder = fld.EntryID;
            settings.ContactsOutlookStore = fld.StoreID;
            BindTo();
        }

        private void btnSelectOutlookCalendarFolder_Click(object sender, EventArgs e)
        {
            var fld = app.Session.PickFolder();
            settings.CalendarOutlookFolder = fld.EntryID;
            settings.CalendarOutlookStore = fld.StoreID;
            BindTo();
        }

        private void btnClearContactsFolder_Click(object sender, EventArgs e)
        {
            settings.ContactsIMAPFolder = "";
            settings.ContactsIMAPStore = "";
            BindTo();
        }

        private void btnClearOutlookContactsFolder_Click(object sender, EventArgs e)
        {
            settings.ContactsOutlookFolder = "";
            settings.ContactsOutlookStore = "";
            BindTo();
        }

        private void btnClearCalendarFolder_Click(object sender, EventArgs e)
        {
            settings.CalendarIMAPFolder = "";
            settings.CalendarIMAPStore = "";
            BindTo();
        }

        private void btnClearOutlookCalendarFolder_Click(object sender, EventArgs e)
        {
            settings.CalendarOutlookFolder = "";
            settings.CalendarOutlookStore = "";
            BindTo();
        }
    }
}
