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

namespace OutlookKolab.Debugging
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using OutlookKolab.Kolab;
    using Outlook = Microsoft.Office.Interop.Outlook;
    using System.IO;
    using System.Runtime.InteropServices;

    public partial class EditKolabMessage : Form
    {
        private Outlook.Application app;
        private Outlook.MailItem mail;
        private Outlook.MAPIFolder folder;

        private EditKolabMessage(Outlook.Application app, Outlook.MailItem mail, Outlook.MAPIFolder folder)
        {
            if (app == null) throw new ArgumentNullException("app");
            if (folder == null) throw new ArgumentNullException("folder");

            this.app = app;
            this.mail = mail;
            this.folder = folder;

            InitializeComponent();
        }

        public static void Show(Outlook.Application app, Outlook.MAPIFolder folder)
        {
            using (var dlg = new EditKolabMessage(app, null, folder))
            {
                dlg.ShowDialog();
            }
        }

        public static void Show(Outlook.Application app, Outlook.MailItem mail)
        {
            if (mail == null) throw new ArgumentNullException("mail");
            using (var dlg = new EditKolabMessage(app, mail, mail.Parent as Outlook.MAPIFolder))
            {
                dlg.ShowDialog();
            }
        }

        private void EditKolabMessage_Load(object sender, EventArgs e)
        {
            if (mail != null)
            {
                txtSubject.Text = mail.Subject;
                txtBody.Text = mail.Body;
                try
                {
                    Outlook.Attachment a = mail.Attachments.Cast<Outlook.Attachment>().FirstOrDefault();
                    if (a != null)
                    {
                        txtMimeType.Text = a.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x370E001F") as string;
                        txtXml.Text = mail.ExtractXml();
                    }
                    else
                    {
                        txtMimeType.Text = "application/x-vnd.kolab.contact || application/x-vnd.kolab.event";
                    }
                }
                catch (SyntaxErrorException ex)
                {
                    txtXml.Text = ex.Message;
                }
            }
            else
            {
                txtSubject.Text = string.Format("dbg-{0}", Guid.NewGuid());
                txtBody.Text = "New Debug Kolab Message";
                txtXml.Text = "some xml";
                txtMimeType.Text = "application/x-vnd.kolab.contact || application/x-vnd.kolab.event";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Create & Upload new Message  
                // IMAP needs a new Message uploaded
                Outlook.MailItem msgToDelete = null;
                if (mail != null)
                {
                    msgToDelete = (Outlook.MailItem)folder.Items[mail.Subject];
                    Marshal.ReleaseComObject(mail);
                }

                // Create the new message
                Outlook.MailItem result = (Outlook.MailItem)folder.Items.Add(Outlook.OlItemType.olMailItem);
                // Set the easy parts of the message
                result.Subject = txtSubject.Text;
                result.Body = txtBody.Text;

                // Save the XML File in a Temp file
                // TODO: Call the little C++ helper to store the attachment directly
                var tmpfilename = Path.GetTempFileName();
                var filename = Path.Combine(Path.GetDirectoryName(tmpfilename), Path.GetFileNameWithoutExtension(tmpfilename)) + ".xml";
                using (var f = File.CreateText(filename))
                {
                    f.Write(txtXml.Text);
                }
                // Create the attachment
                var a = result.Attachments.Add(filename, Outlook.OlAttachmentType.olByValue, 1, "kolab.xml");
                // Use Trick #17 to set the correct MimeType
                a.PropertyAccessor.SetProperty("http://schemas.microsoft.com/mapi/proptag/0x370E001F", txtMimeType.Text);

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
                result.Move(folder);

                // Delete temp. file
                File.Delete(filename);
                if (msgToDelete != null)
                {
                    msgToDelete.Delete();
                    Marshal.ReleaseComObject(msgToDelete);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Unable to save message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
