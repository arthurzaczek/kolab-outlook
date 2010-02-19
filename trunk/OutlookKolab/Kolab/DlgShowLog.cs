﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OutlookKolab.Kolab
{
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
            dsStatus1.StatusEntry.Rows.Clear();
            dsStatus1.AcceptChanges();
            dsStatus1.Save();
            dsStatus1.ReLoad();
        }
    }
}