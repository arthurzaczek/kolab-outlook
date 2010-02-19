using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using Microsoft.Office.Interop.Outlook;
using OutlookKolab.Kolab;

namespace OutlookKolab
{
    public partial class ThisAddIn
    {
        #region Properties/Fields
        Office.CommandBar toolBar;
        Office.CommandBarButton settingsButton;
        Office.CommandBarButton syncButton;
        Office.CommandBarButton logButton;
        Office.CommandBarButton statusButton;
        private const string SYNC_ROOT = "ThisAddIn_SYNC_ROOT";
        #endregion

        #region ToolBar
        private Office.CommandBar FindToolBar(Office.CommandBars cmdBars, string name)
        {
            foreach (var tb in cmdBars.OfType<Office.CommandBar>())
            {
                if (tb.Name == name)
                {
                    return tb;
                }
            }

            return null;
        }

        private Office.CommandBarButton FindButton(Office.CommandBar cmdBar, string name)
        {
            foreach (var bt in cmdBar.Controls.OfType<Office.CommandBarButton>())
            {
                if (bt.Caption == name)
                {
                    return bt;
                }
            }

            return null;
        }

        private void CreateToolbar()
        {
            Office.CommandBars cmdBars = this.Application.ActiveExplorer().CommandBars;

            toolBar = FindToolBar(cmdBars, "Sync Kolab");
            if (toolBar == null)
            {
                toolBar = cmdBars.Add("Sync Kolab", Office.MsoBarPosition.msoBarTop, false, false);
            }

            // Static buttons
            settingsButton = FindButton(toolBar, "Settings");
            if (settingsButton == null)
            {
                settingsButton = (Office.CommandBarButton)toolBar.Controls.Add(1, missing, missing, missing, false);
                settingsButton.Style = Office.MsoButtonStyle.msoButtonCaption;
                settingsButton.Caption = "Settings";
            }
            settingsButton.Click += new Office._CommandBarButtonEvents_ClickEventHandler(settingsButton_Click);

            logButton = FindButton(toolBar, "Log");
            if (logButton == null)
            {
                logButton = (Office.CommandBarButton)toolBar.Controls.Add(1, missing, missing, missing, false);
                logButton.Style = Office.MsoButtonStyle.msoButtonCaption;
                logButton.Caption = "Log";
            }
            logButton.Click += new Office._CommandBarButtonEvents_ClickEventHandler(logButton_Click);

            // Temp buttons
            syncButton = (Office.CommandBarButton)toolBar.Controls.Add(1, missing, missing, missing, true);
            syncButton.Style = Office.MsoButtonStyle.msoButtonCaption;
            syncButton.Caption = "Sync";
            syncButton.Click += new Office._CommandBarButtonEvents_ClickEventHandler(syncButton_Click);

            statusButton = (Office.CommandBarButton)toolBar.Controls.Add(1, missing, missing, missing, true);
            statusButton.Style = Office.MsoButtonStyle.msoButtonCaption;
            statusButton.Caption = "Idle";
            statusButton.Click += new Office._CommandBarButtonEvents_ClickEventHandler(logButton_Click);
        }
        #endregion

        #region Start/Stop
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            CreateToolbar();
            StatusHandler.SyncStatus += new SyncStatusHandler(StatusHandler_SyncStatus);
            StatusHandler.SyncStarted += new SyncNotifyHandler(StatusHandler_SyncStarted);
            StatusHandler.SyncFinished += new SyncNotifyHandler(StatusHandler_SyncFinished);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            lock (SYNC_ROOT)
            {
                statusButton = null;
                syncButton = null;
                logButton = null;
                settingsButton = null;
            }
            OutlookKolab.Kolab.Sync.SyncWorker.Stop();
        }
        #endregion

        #region StatusEvents
        void StatusHandler_SyncFinished()
        {
            lock (SYNC_ROOT)
            {
                if (statusButton != null && syncButton != null)
                {
                    syncButton.Caption = "Sync";
                }
            }
        }

        void StatusHandler_SyncStarted()
        {
            lock (SYNC_ROOT)
            {
                if (statusButton != null && syncButton != null)
                {
                    syncButton.Caption = "Stop";
                }
            }
        }

        void StatusHandler_SyncStatus(string text)
        {
            lock (SYNC_ROOT)
            {
                if (statusButton != null)
                {
                    statusButton.Caption = text;
                }
            }
        }

        #endregion

        #region ButtonEvents
        private void settingsButton_Click(Office.CommandBarButton ctrl, ref bool cancel)
        {
            OutlookKolab.Kolab.Settings.DlgSettings.Show(this.Application);
        }

        private void logButton_Click(Office.CommandBarButton ctrl, ref bool cancel)
        {
            OutlookKolab.Kolab.DlgShowLog.Show();
        }

        private void syncButton_Click(Office.CommandBarButton ctrl, ref bool cancel)
        {
            if (OutlookKolab.Kolab.Sync.SyncWorker.IsRunning)
            {
                syncButton.Caption = "Stopping";
                OutlookKolab.Kolab.Sync.SyncWorker.Stop();
            }
            else
            {
                var worker = new OutlookKolab.Kolab.Sync.SyncWorker(this.Application);
                worker.Start();
            }
        }
        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
