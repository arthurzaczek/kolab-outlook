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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using Outlook = Microsoft.Office.Interop.Outlook;

    public partial class DlgConflictDialog : Form
    {
        private List<SyncContext> conflictList;
        private ISyncHandler handler;
        private Outlook.Folder imapFolder;

        public DlgConflictDialog(ISyncHandler handler, Outlook.Folder imapFolder, List<SyncContext> conflictList)
        {
            this.conflictList = conflictList;
            this.handler = handler;
            this.imapFolder = imapFolder;
            InitializeComponent();
            EnableButtons(false);
            BindTo();
        }

        public static void Show(ISyncHandler handler, Outlook.Folder imapFolder, List<SyncContext> conflictList)
        {
            try
            {
                using (var dlg = new DlgConflictDialog(handler, imapFolder, conflictList))
                {
                    dlg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var sync = GetCurrentSelectedItem();
                if (sync != null)
                {
                    if (sync.LocalItem != null)
                    {
                        txtLocal.Text = handler.getMessageBodyText(sync);
                    }
                    else
                    {
                        txtLocal.Text = "DELETED";
                    }

                    txtRemote.Text = sync.Message.Body;

                    EnableButtons(true);
                }
                else
                {
                    txtLocal.Text = "";
                    txtRemote.Text = "";
                    EnableButtons(false);
                }
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private SyncContext GetCurrentSelectedItem()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                return (SyncContext)dataGridView1.SelectedRows[0].DataBoundItem;
            }
            else
            {
                return null;
            }
        }

        private void BindTo()
        {
            bindingSource1.DataSource = conflictList;
            lbStatus.Text = string.Format("{0} conflicting items", conflictList.Count);
        }

        private void EnableButtons(bool enable)
        {
            btnUseLocal.Enabled = enable;
            btnUseAllLocal.Enabled = enable;
            btnUseRemote.Enabled = enable;
            btnUseAllRemote.Enabled = enable;
        }

        private void UseLocal(SyncContext sync)
        {
            if (sync.LocalItem == null)
            {
                handler.deleteServerItem(sync);
            }
            else
            {
                handler.updateServerItemFromLocal(imapFolder, sync);
            }
        }

        private void UseRemote(SyncContext sync)
        {
            handler.updateLocalItemFromServer(sync);
        }

        private void btnUseLocal_Click(object sender, EventArgs e)
        {
            try
            {
                var sync = GetCurrentSelectedItem();
                if (sync == null) return;
                UseLocal(sync);
                conflictList.Remove(sync);
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnUseAllLocal_Click(object sender, EventArgs e)
        {
            try
            {
                int counter = 0;
                foreach (var sync in conflictList)
                {
                    UseLocal(sync);
                    lbStatus.Text = string.Format("processing {0}/{1}", ++counter, conflictList.Count);
                    Application.DoEvents();
                }
                conflictList.Clear();
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnUseRemote_Click(object sender, EventArgs e)
        {
            try
            {
                var sync = GetCurrentSelectedItem();
                if (sync == null) return;
                UseRemote(sync);
                conflictList.Remove(sync);
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void btnUseAllRemote_Click(object sender, EventArgs e)
        {
            try
            {
                int counter = 0;
                foreach (var sync in conflictList.ToArray())
                {
                    UseRemote(sync);
                    lbStatus.Text = string.Format("processing {0}/{1}", ++counter, conflictList.Count);
                    Application.DoEvents();
                }
                conflictList.Clear();
                BindTo();
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }
    }
}
