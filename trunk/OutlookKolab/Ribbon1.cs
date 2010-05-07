using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace OutlookKolab
{
    public partial class RibbonSyncKolab
    {
        private ThisAddIn addIn;
        public RibbonSyncKolab(ThisAddIn addIn)
            : this()
        {
            this.addIn = addIn;
        }

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void buttonSettings_Click(object sender, RibbonControlEventArgs e)
        {
            OutlookKolab.Kolab.Settings.DlgSettings.Show(addIn.Application);
        }

        private void buttonLog_Click(object sender, RibbonControlEventArgs e)
        {
            OutlookKolab.Kolab.DlgShowLog.Show();
        }

        private void buttonSync_Click(object sender, RibbonControlEventArgs e)
        {
            addIn.Sync();
        }

        private void buttonStatus_Click(object sender, RibbonControlEventArgs e)
        {
            OutlookKolab.Kolab.DlgShowLog.Show();
        }
       
    }
}
