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

namespace OutlookKolab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml.Linq;

    using Microsoft.Office.Interop.Outlook;
    using OutlookKolab.Kolab;
    using Office = Microsoft.Office.Core;
    using Outlook = Microsoft.Office.Interop.Outlook;

    public partial class ThisAddIn
    {
        protected override void Dispose(bool disposing)
        {
            if (ribbon != null) ribbon.Dispose();
            ribbon = null;

            if (ribbonMgr != null) ribbonMgr.Dispose();
            ribbonMgr = null;

            base.Dispose(disposing);
        }

        #region Properties/Fields
        private static readonly object _lock = new object();

        Office.CommandBar toolBar;
        Office.CommandBarButton settingsButton;
        Office.CommandBarButton syncButton;
        Office.CommandBarButton logButton;
        Office.CommandBarButton statusButton;

        RibbonSyncKolab ribbon;
        Microsoft.Office.Tools.Ribbon.RibbonManager ribbonMgr;

        System.Windows.Forms.Timer timer = null;
        //private readonly int timerDueTime = 10000; // 10 seconds
        private readonly int timerPeriod = 1000 * 60 * 30; // every half hour, TODO: Configure
        #endregion

        #region ToolBar
        /// <summary>
        /// Finds a toolbar by name
        /// </summary>
        /// <param name="cmdBars">Outlooks CommandBar collection</param>
        /// <param name="name">Button Name</param>
        /// <returns>The CommandBar or null if not found</returns>
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

        /// <summary>
        /// Finds a button by caption
        /// </summary>
        /// <param name="cmdBar">Outlooks CommandBar</param>
        /// <param name="name">Buttons caption</param>
        /// <returns>The Button or null if not found</returns>
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

        /// <summary>
        /// Create/Update the toolbar
        /// </summary>
        private void CreateToolbar()
        {
            Office.CommandBars cmdBars = this.Application.ActiveExplorer().CommandBars;

            // find/create toolbar
            toolBar = FindToolBar(cmdBars, "Sync Kolab");
            if (toolBar == null)
            {
                toolBar = cmdBars.Add("Sync Kolab", Office.MsoBarPosition.msoBarTop, false, false);
            }

            // --- Static buttons ---
            // Settings button
            settingsButton = FindButton(toolBar, "Settings");
            if (settingsButton == null)
            {
                settingsButton = (Office.CommandBarButton)toolBar.Controls.Add(1, missing, missing, missing, false);
                settingsButton.Style = Office.MsoButtonStyle.msoButtonCaption;
                settingsButton.Caption = "Settings";
            }
            settingsButton.Click += new Office._CommandBarButtonEvents_ClickEventHandler(settingsButton_Click);

            // Log button
            logButton = FindButton(toolBar, "Log");
            if (logButton == null)
            {
                logButton = (Office.CommandBarButton)toolBar.Controls.Add(1, missing, missing, missing, false);
                logButton.Style = Office.MsoButtonStyle.msoButtonCaption;
                logButton.Caption = "Log";
            }
            logButton.Click += new Office._CommandBarButtonEvents_ClickEventHandler(logButton_Click);

            // --- Temp buttons ---
            // create those buttons temporary because their caption will change on click events
            // Sync button
            syncButton = (Office.CommandBarButton)toolBar.Controls.Add(1, missing, missing, missing, true);
            syncButton.Style = Office.MsoButtonStyle.msoButtonCaption;
            syncButton.Caption = "Sync";
            syncButton.Click += new Office._CommandBarButtonEvents_ClickEventHandler(syncButton_Click);

            // Status button
            statusButton = (Office.CommandBarButton)toolBar.Controls.Add(1, missing, missing, missing, true);
            statusButton.Style = Office.MsoButtonStyle.msoButtonCaption;
            statusButton.Caption = "Idle";
            statusButton.Click += new Office._CommandBarButtonEvents_ClickEventHandler(logButton_Click);
        }
        #endregion

        #region Ribbon
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            ribbon = new RibbonSyncKolab(this);
            ribbonMgr = new Microsoft.Office.Tools.Ribbon.RibbonManager(new Microsoft.Office.Tools.Ribbon.OfficeRibbon[] { ribbon });
            return ribbonMgr;
        }
        #endregion

        #region Start/Stop
        /// <summary>
        /// AddIn Startup code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Create Toolbar
            CreateToolbar();

            // Register Sync Status handler
            StatusHandler.SyncStatus += new SyncStatusHandler(StatusHandler_SyncStatus);
            StatusHandler.SyncStarted += new SyncNotifyHandler(StatusHandler_SyncStarted);
            StatusHandler.SyncFinished += new SyncNotifyHandler(StatusHandler_SyncFinished);

            // Register timer
            timer = new System.Windows.Forms.Timer();
            timer.Interval = timerPeriod;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            // Sync now
            Sync();
        }


        /// <summary>
        /// AddIn Shutdown code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Remove buttons
            lock (_lock)
            {
                statusButton = null;
                syncButton = null;
                logButton = null;
                settingsButton = null;
            }

            // Stop a running sync
            OutlookKolab.Kolab.Sync.SyncWorker.Stop();
        }
        #endregion

        #region Timer
        void timer_Tick(object sender, EventArgs e)
        {
            Sync();
        }
        #endregion

        #region StatusEvents
        void StatusHandler_SyncFinished()
        {
            lock (_lock)
            {
                if (statusButton != null && syncButton != null)
                {
                    SetSyncButtonText("Sync");
                }
            }
        }

        void StatusHandler_SyncStarted()
        {
            lock (_lock)
            {
                if (statusButton != null && syncButton != null)
                {
                    SetSyncButtonText("Stop");
                }
            }
        }

        void StatusHandler_SyncStatus(string text)
        {
            lock (_lock)
            {
                if (statusButton != null)
                {
                    SetStatusButtonText(text);
                }
            }
        }

        private void SetStatusButtonText(string text)
        {
            statusButton.Caption = text;
            if (ribbon != null && ribbon.buttonStatus != null)
            {
                ribbon.buttonStatus.Label = text;
            }
        }

        private void SetSyncButtonText(string text)
        {
            syncButton.Caption = text;
            if (ribbon != null && ribbon.buttonSync != null)
            {
                ribbon.buttonSync.Label = text;
            }
        }
        #endregion

        #region ButtonEvents
        /// <summary>
        /// Settings Button - shows settings dialog
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="cancel"></param>
        private void settingsButton_Click(Office.CommandBarButton ctrl, ref bool cancel)
        {
            OutlookKolab.Kolab.Settings.DlgSettings.Show(this.Application);
        }

        /// <summary>
        /// Log Button - shows log dialog
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="cancel"></param>
        private void logButton_Click(Office.CommandBarButton ctrl, ref bool cancel)
        {
            OutlookKolab.Kolab.DlgShowLog.Show();
        }

        /// <summary>
        /// Sync Buttton - starts a sync if not running, stops a sync if running.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="cancel"></param>
        private void syncButton_Click(Office.CommandBarButton ctrl, ref bool cancel)
        {
            Sync();
        }

        public void Sync()
        {
            if (OutlookKolab.Kolab.Sync.SyncWorker.IsRunning)
            {
                SetStatusButtonText("Stopping");
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
