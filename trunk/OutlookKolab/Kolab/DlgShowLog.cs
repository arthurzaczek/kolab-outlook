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

namespace OutlookKolab.Kolab
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using OutlookKolab.Kolab.Provider;

    public partial class DlgShowLog : Form
    {
        private DlgShowLog()
        {
            InitializeComponent();
            dsStatus1.ReLoad();
        }

        public static new void Show()
        {
            DlgShowLog dlg = new DlgShowLog();
            dlg.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DlgShowLog_Load(object sender, EventArgs e)
        {
            var last = dataGridView1.Rows.Cast<DataGridViewRow>().LastOrDefault();
            if (last != null)
            {
                dataGridView1.CurrentCell = last.Cells[0];
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            foreach (var r in dsStatus1.StatusEntry.Rows.Cast<DSStatus.StatusEntryRow>().ToList())
            {
                r.Delete();
            }
            dsStatus1.AcceptChanges();
            dsStatus1.Save();
            dsStatus1.ReLoad();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var row = dsStatus1.StatusEntry[e.RowIndex];
            if (row.errors > 0)
            {
                e.CellStyle.BackColor = Color.LightCoral;
            }
        }
    }
}
