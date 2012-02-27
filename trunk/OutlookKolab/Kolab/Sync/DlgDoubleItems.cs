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

    public partial class DlgDoubleItems : Form
    {
        private List<SyncContext> doubleItemsList;
        private ISyncHandler handler;
        private Outlook.Folder imapFolder;

        public DlgDoubleItems(ISyncHandler handler, Outlook.Folder imapFolder, List<SyncContext> doubleItemsList)
        {
            this.doubleItemsList = doubleItemsList;
            this.handler = handler;
            this.imapFolder = imapFolder;

            InitializeComponent();

            BindTo();
        }

        public static void Show(ISyncHandler handler, Outlook.Folder imapFolder, List<SyncContext> conflictList)
        {
            try
            {
                using (var dlg = new DlgDoubleItems(handler, imapFolder, conflictList))
                {
                    dlg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Helper.HandleError(ex);
            }
        }

        private void BindTo()
        {
            var sb = new StringBuilder();
            foreach (var item in doubleItemsList)
            {
                sb.AppendLine(item.Message.Subject);
            }

            txt.Text = sb.ToString();
        }
    }
}
