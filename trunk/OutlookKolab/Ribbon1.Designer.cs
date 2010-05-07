namespace OutlookKolab
{
    partial class RibbonSyncKolab : Microsoft.Office.Tools.Ribbon.OfficeRibbon
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public RibbonSyncKolab()
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabSyncKolab = new Microsoft.Office.Tools.Ribbon.RibbonTab();
            this.group1 = new Microsoft.Office.Tools.Ribbon.RibbonGroup();
            this.buttonSettings = new Microsoft.Office.Tools.Ribbon.RibbonButton();
            this.buttonLog = new Microsoft.Office.Tools.Ribbon.RibbonButton();
            this.buttonSync = new Microsoft.Office.Tools.Ribbon.RibbonButton();
            this.buttonStatus = new Microsoft.Office.Tools.Ribbon.RibbonButton();
            this.tabSyncKolab.SuspendLayout();
            this.group1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSyncKolab
            // 
            this.tabSyncKolab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabSyncKolab.Groups.Add(this.group1);
            this.tabSyncKolab.Name = "tabSyncKolab";
            // 
            // group1
            // 
            this.group1.Items.Add(this.buttonSettings);
            this.group1.Items.Add(this.buttonLog);
            this.group1.Items.Add(this.buttonSync);
            this.group1.Items.Add(this.buttonStatus);
            this.group1.Label = "Kolab";
            this.group1.Name = "group1";
            // 
            // buttonSettings
            // 
            this.buttonSettings.Label = "Settings";
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Click += new System.EventHandler<Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs>(this.buttonSettings_Click);
            // 
            // buttonLog
            // 
            this.buttonLog.Label = "Log";
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Click += new System.EventHandler<Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs>(this.buttonLog_Click);
            // 
            // buttonSync
            // 
            this.buttonSync.Label = "Sync";
            this.buttonSync.Name = "buttonSync";
            this.buttonSync.Click += new System.EventHandler<Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs>(this.buttonSync_Click);
            // 
            // buttonStatus
            // 
            this.buttonStatus.Label = "Idle";
            this.buttonStatus.Name = "buttonStatus";
            this.buttonStatus.Click += new System.EventHandler<Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs>(this.buttonStatus_Click);
            // 
            // RibbonSyncKolab
            // 
            this.Name = "RibbonSyncKolab";
            this.RibbonType = "Microsoft.Outlook.Explorer";
            this.Tabs.Add(this.tabSyncKolab);
            this.Load += new System.EventHandler<Microsoft.Office.Tools.Ribbon.RibbonUIEventArgs>(this.Ribbon1_Load);
            this.tabSyncKolab.ResumeLayout(false);
            this.tabSyncKolab.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabSyncKolab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonLog;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonSync;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonStatus;
    }

    partial class ThisRibbonCollection : Microsoft.Office.Tools.Ribbon.RibbonReadOnlyCollection
    {
        internal RibbonSyncKolab Ribbon1
        {
            get { return this.GetRibbon<RibbonSyncKolab>(); }
        }
    }
}
