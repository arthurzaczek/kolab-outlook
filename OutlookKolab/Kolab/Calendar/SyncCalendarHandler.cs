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

namespace OutlookKolab.Kolab.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;

    using OutlookKolab.Kolab.Provider;
    using OutlookKolab.Kolab.Settings;
    using OutlookKolab.Kolab.Sync;
    using Outlook = Microsoft.Office.Interop.Outlook;

    /// <summary>
    /// Calendar sync handler
    /// </summary>
    public class SyncCalendarHandler : AbstractSyncHandler
    {
        /// <summary>
        /// Local cache provider
        /// </summary>
        LocalCacheProvider cache = null;

        /// <summary>
        /// Creates a new calendar sync handler
        /// </summary>
        /// <param name="settings">current settings</param>
        /// <param name="dsStatus">current row</param>
        /// <param name="app">Outlook Application Object</param>
        public SyncCalendarHandler(DSSettings settings, DSStatus dsStatus, Outlook.Application app)
            : base(settings, dsStatus, app)
        {
            cache = new LocalCacheProvider(LocalCacheProviderType.Calendar);
            status.task = "Calendar";
        }

        /// <summary>
        /// Returns all Entry IDs of all local items
        /// </summary>
        /// <returns>List of Entry IDs</returns>
        public override IList<string> getAllLocalItemIDs()
        {
            var lst = Folder.Items.OfType<Outlook.AppointmentItem>().ToList();
            var result = lst.Select(i => i.EntryID).ToList();
            lst.ForEach(i => Marshal.ReleaseComObject(i));
            return result;
        }

        /// <summary>
        /// Current handlers IMAP Folder Entry ID = Remote Items
        /// </summary>
        /// <returns>Entry ID</returns>
        public override string GetIMAPFolderName()
        {
            return settings.Settings[0].CalendarIMAPFolder;
        }
        /// <summary>
        /// Current handlers IMAP Folder Store ID = Remote Items
        /// </summary>
        /// <returns>Store ID</returns>
        public override string GetIMAPStoreID()
        {
            return settings.Settings[0].CalendarIMAPStore;
        }

        /// <summary>
        /// Current handlers local Folder Entry ID = Local Items
        /// </summary>
        /// <returns>Entry ID</returns>
        public override string GetOutlookFolderName()
        {
            return settings.Settings[0].CalendarOutlookFolder;
        }
        /// <summary>
        /// Current handlers local Folder Store ID = Local Items
        /// </summary>
        /// <returns>Store ID</returns>
        public override string GetOutlookStoreID()
        {
            return settings.Settings[0].CalendarOutlookStore;
        }

        /// <summary>
        /// Returns the local cache provider of the current handler
        /// </summary>
        /// <returns>LocalCacheProvider</returns>
        public override LocalCacheProvider getLocalCacheProvider()
        {
            return cache;
        }

        /// <summary>
        /// Retreives a local calendar item. SyncContext will be updated
        /// </summary>
        /// <param name="sync">current sync context.</param>
        /// <returns>Outlook.AppointmentItem or null if not found or item was deleted or moved</returns>
        private Outlook.AppointmentItem getLocalItem(SyncContext sync)
        {
            if (sync.LocalItem != null) return (Outlook.AppointmentItem)sync.LocalItem;
            Outlook.AppointmentItem result = null;
            try
            {
                result = app.Session.GetItemFromID(sync.CacheEntry.localId, Folder.StoreID) as Outlook.AppointmentItem;
                var fld = (Outlook.Folder)result.Parent;
                if (fld.FolderPath != Folder.FolderPath)
                {
                    // Has been deleted or moved
                    return null;
                }
            }
            catch // TODO: 
            {
                result = null;
            }
            sync.LocalItem = result;
            return result;
        }

        /// <summary>
        /// Computes the local hash of the given Outlook AppointmentItem
        /// </summary>
        /// <param name="item">Outlook AppointmentItem</param>
        /// <returns>Hash as string</returns>
        private String getLocalHash(Outlook.AppointmentItem item)
        {
            List<String> contents = new List<String>();

            // Android part
            contents.Add(item.Subject ?? "no Subject");
            contents.Add(item.StartUTC.ToOADate().ToString());
            contents.Add(item.EndUTC.ToOADate().ToString());
            contents.Add(item.AllDayEvent.ToString());
            contents.Add(item.Body ?? "no Body");
            contents.Add(item.Location ?? "no Location");

            // the rest
            contents.Add(item.BusyStatus.ToString());
            contents.Add(item.Categories ?? "no Categories");
            contents.Add(item.Duration.ToString());
            contents.Add(item.Organizer ?? "no Organizer");
            contents.Add(item.ReminderSet.ToString());
            contents.Add(item.ReminderMinutesBeforeStart.ToString());
            contents.Add(item.Sensitivity.ToString());

            // Recurring
            contents.Add(item.IsRecurring.ToString());
            if (item.IsRecurring)
            {
                var pattern = item.GetRecurrencePattern();
                contents.Add(pattern.DayOfMonth.ToString());
                contents.Add(pattern.DayOfWeekMask.ToString());
                contents.Add(pattern.Duration.ToString());
                contents.Add(pattern.EndTime.ToOADate().ToString());
                contents.Add(pattern.Instance.ToString());
                contents.Add(pattern.Interval.ToString());
                contents.Add(pattern.MonthOfYear.ToString());
                contents.Add(pattern.NoEndDate.ToString());
                contents.Add(pattern.Occurrences.ToString());
                contents.Add(pattern.PatternEndDate.ToOADate().ToString());
                contents.Add(pattern.PatternStartDate.ToOADate().ToString());
                contents.Add(pattern.RecurrenceType.ToString());
                contents.Add(pattern.Regenerate.ToString());
                contents.Add(pattern.StartTime.ToOADate().ToString());
            }

            return String.Join("|", contents.ToArray());
        }

        /// <summary>
        /// checks for local changes
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>true if the local item changes since last sync</returns>
        public override bool hasLocalChanges(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Checking for local changes: #" + sync.CacheEntry.localId);
            var cal = getLocalItem(sync);
            string entryHash = sync.CacheEntry.localHash;
            string contactHash = cal != null ? getLocalHash(cal) : "";
            return entryHash != contactHash;
        }

        /// <summary>
        /// checks if the local item exits
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>true if the local item exits</returns>
        public override bool hasLocalItem(SyncContext sync)
        {
            return getLocalItem(sync) != null;
        }

        /// <summary>
        /// update or create the Kolab XML for a server item from a given local item and server xml
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <param name="xml">actual Kolab XML</param>
        /// <returns>new/updated Kolab XML</returns>
        protected override string updateServerItemFromLocal(SyncContext sync, string xml)
        {
            var source = getLocalItem(sync);
            sync.CacheEntry.localHash = getLocalHash(source);
            sync.CacheEntry.remoteChangedDate = DateTime.Now.ToUniversalTime();

            var cal = Xml.XmlHelper.ParseCalendar(xml);
            sync.CacheEntry.remoteId = cal.uid ?? getNewUid();
            return writeXml(source, cal, sync.CacheEntry.remoteChangedDate);
        }

        /// <summary>
        /// Deletes a local item
        /// </summary>
        /// <param name="localId">Entry ID</param>
        protected override void deleteLocalItem(string localId)
        {
            var e = app.Session.GetItemFromID(localId, Folder.StoreID) as Outlook.AppointmentItem;
            if (e != null) e.Delete();
        }

        /// <summary>
        /// Returns the Kolab Mime Type
        /// </summary>
        /// <returns>Mime Type - application/x-vnd.kolab.event</returns>
        protected override string getMimeType()
        {
            return "application/x-vnd.kolab.event";
        }

        /// <summary>
        /// Update or create a local item from a given server item
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <param name="xml">Kolab XML representing the server item</param>
        protected override void updateLocalItemFromServer(SyncContext sync, string xml)
        {
            // Parse calendar item from given xml
            Xml.@event cal = null;
            try
            {
                cal = Xml.XmlHelper.ParseCalendar(xml);
            }
            catch (Exception ex)
            {
                // Unable to parse -> abort
                throw new SyncException(GetItemText(sync), "Unable to parse XML Document", ex);
            }

            // Get or add local item
            var localCal = (Outlook.AppointmentItem)sync.LocalItem;
            if (localCal == null)
            {
                localCal = (Outlook.AppointmentItem)Folder.Items.Add(Outlook.OlItemType.olAppointmentItem);
            }

            // Remember reccuring
            bool isRecurring = cal.recurrence != null && !string.IsNullOrEmpty(cal.recurrence.cycle);
            bool isAllDay = cal.startdate.TimeOfDay == TimeSpan.Zero && cal.enddate.TimeOfDay == TimeSpan.Zero;

            try
            {
                // Android part
                localCal.Subject = cal.summary;
                if (!isRecurring)
                {
                    localCal.StartUTC = cal.startdate.ToUniversalTime();
                    localCal.EndUTC = isAllDay ? cal.enddate.ToUniversalTime().AddDays(1) : cal.enddate.ToUniversalTime();
                    localCal.AllDayEvent = isAllDay;
                }
                localCal.Body = cal.body;
                localCal.Location = cal.location;

                // the rest
                localCal.BusyStatus = cal.GetBusyStatus();
                localCal.Categories = cal.categories;
                // localCal.Duration = calculated by start/end;
                // localCal.Organizer = cal.organizer != null ? cal.organizer.displayname : string.Empty; ReadOnly ???
                if (cal.alarm > 0)
                {
                    localCal.ReminderSet = true;
                    localCal.ReminderMinutesBeforeStart = cal.alarm;
                }
                else
                {
                    localCal.ReminderSet = false;
                }
                localCal.Sensitivity = cal.GetSensitivity();
            }
            catch (COMException ex)
            {
                // Troubles setting properties -> abort
                throw new SyncException(GetItemText(sync), "Unable to set basic AppointmentItem options", ex);
            }
            catch (ArgumentException ex)
            {
                // Troubles setting properties -> abort
                throw new SyncException(GetItemText(sync), "Unable to set basic AppointmentItem options", ex);
            }

            // Recurring
            if (isRecurring)
            {
                try
                {
                    // Get or create RecurrencePattern
                    var pattern = localCal.GetRecurrencePattern();

                    // Depending on RecurrenceType
                    pattern.RecurrenceType = cal.GetRecurrenceType();

                    // Time
                    pattern.PatternStartDate = cal.startdate.Date;
                    // pattern.PatternEndDate = set by range
                    DateTime startDate = cal.startdate;
                    if (startDate.Kind == DateTimeKind.Utc) startDate = startDate.ToLocalTime();
                    var startTime = startDate.TimeOfDay;

                    pattern.StartTime = DateTime.MinValue + startTime;

                    if (cal.enddate > new DateTime(1980, 1, 1))
                    {
                        DateTime endDate = cal.enddate;
                        if (endDate.Kind == DateTimeKind.Utc) endDate = endDate.ToLocalTime();
                        var endTime = startDate.TimeOfDay;

                        var duration = (isAllDay ? endDate.AddDays(1) : endDate) - startDate;

                        pattern.EndTime = DateTime.MinValue + endTime;
                        pattern.Duration = (int)duration.TotalMinutes;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Found recurring calendar entry without end time: " + cal.ToString());
                    }

                    // Only if valid or not yearly - only outlook 2007 & 2010 does support yearly recurrences
                    if (cal.recurrence.interval != 0 &&
                        pattern.RecurrenceType != Microsoft.Office.Interop.Outlook.OlRecurrenceType.olRecursYearly &&
                        pattern.RecurrenceType != Microsoft.Office.Interop.Outlook.OlRecurrenceType.olRecursYearNth)
                    {
                        pattern.Interval = cal.recurrence.interval;
                    }
                    else
                    {
                        // Ignore Intervals on Years - only Outlook 2007 uses that. 
                        // Outlook < 2007 does not set this property. 
                        // Even worse - Outlook 2007 and 2010 sets the interval value in Months (!) not in years!
                        //pattern.Interval = 1;
                    }

                    // set properties dependeing on recurrence type
                    switch (pattern.RecurrenceType)
                    {
                        case Outlook.OlRecurrenceType.olRecursDaily:
                            break;
                        case Outlook.OlRecurrenceType.olRecursWeekly:
                            if (cal.recurrence.day != null) pattern.DayOfWeekMask = cal.GetDayOfWeekMask();
                            break;
                        case Outlook.OlRecurrenceType.olRecursMonthNth:
                            if (cal.recurrence.day != null) pattern.DayOfWeekMask = cal.GetDayOfWeekMask();
                            pattern.Instance = cal.recurrence.daynumber;
                            break;
                        case Outlook.OlRecurrenceType.olRecursMonthly:
                            if (cal.recurrence.daynumber != 0)
                            {
                                pattern.DayOfMonth = cal.recurrence.daynumber;
                            }
                            break;
                        case Outlook.OlRecurrenceType.olRecursYearNth:
                            pattern.MonthOfYear = cal.GetMonthOfYear();
                            pattern.Instance = cal.recurrence.daynumber;
                            if (cal.recurrence.day != null) pattern.DayOfWeekMask = cal.GetDayOfWeekMask();
                            break;
                        case Outlook.OlRecurrenceType.olRecursYearly:
                            pattern.DayOfMonth = cal.recurrence.daynumber;
                            pattern.MonthOfYear = cal.GetMonthOfYear();
                            break;
                    }
                    // Set pattern range
                    if (cal.recurrence.range != null && !string.IsNullOrEmpty(cal.recurrence.range.type))
                    {
                        if (cal.recurrence.range.type == "none")
                        {
                            pattern.NoEndDate = true;
                        }
                        else if (cal.recurrence.range.type == "date")
                        {
                            DateTime tmp;
                            if (DateTime.TryParse(cal.recurrence.range.Value, out tmp))
                            {
                                pattern.PatternEndDate = tmp;
                            }
                        }
                        else if (cal.recurrence.range.type == "number")
                        {
                            int tmp;
                            if (int.TryParse(cal.recurrence.range.Value, out tmp))
                            {
                                pattern.Occurrences = tmp;
                            }
                        }
                    }
                }
                catch (COMException ex)
                {
                    // Troubles setting properties -> abort
                    throw new SyncException(GetItemText(sync), "Unable to set AppointmentItem recurrence", ex);
                }
                catch (ArgumentException ex)
                {
                    // Troubles setting properties -> abort
                    throw new SyncException(GetItemText(sync), "Unable to set AppointmentItem recurrence", ex);
                }
            }
            else
            {
                // No recurrence pattern -> clear
                localCal.ClearRecurrencePattern();
            }

            try
            {
                // Save local item
                localCal.Save();
            }
            catch (COMException ex)
            {
                // Troubles saving local item -> abort
                throw new SyncException(GetItemText(sync), "Unable to save AppointmentItem", ex);
            }

            // Create local cache entry if a new item was created
            if (sync.CacheEntry == null)
            {
                sync.CacheEntry = getLocalCacheProvider().createEntry();
            }
            // Upate local cache entry
            sync.CacheEntry.localId = localCal.EntryID;
            sync.CacheEntry.localHash = getLocalHash(localCal);

        }

        /// <summary>
        /// Create Application and Type specific id.
        /// ko == Kolab Outlook, ev == Event
        /// </summary>
        /// <returns>new UID</returns>
        private string getNewUid()
        {
            return "ko-ev-" + Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates a Kolab XML string. This method also must update the local cache entry.
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>xml string</returns>
        protected override string writeXml(SyncContext sync)
        {
            var item = getLocalItem(sync);
            sync.CacheEntry.localHash = getLocalHash(item);
            sync.CacheEntry.remoteChangedDate = DateTime.Now.ToUniversalTime();
            sync.CacheEntry.remoteId = getNewUid();
            return writeXml(item, new OutlookKolab.Kolab.Xml.@event() { uid = sync.CacheEntry.remoteId }, sync.CacheEntry.remoteChangedDate);
        }

        /// <summary>
        /// Creates a Kolab XML string.
        /// </summary>
        /// <param name="source">Outlook Item</param>
        /// <param name="cal">destination calendar XML Object</param>
        /// <param name="lastmodificationdate">last modification date</param>
        /// <returns>xml string</returns>
        private string writeXml(Microsoft.Office.Interop.Outlook.AppointmentItem source, OutlookKolab.Kolab.Xml.@event cal, DateTime lastmodificationdate)
        {
            // Basic properties
            cal.lastmodificationdate = lastmodificationdate;
            cal.summary = source.Subject;
            if (!source.IsRecurring)
            {
                // StartUTC/EndUTC does not specify DateTime.Kind == Utc!
                cal.startdate = source.AllDayEvent ? source.Start.Date : source.Start.ToUniversalTime();
                cal.enddate = source.AllDayEvent ? source.End.Date.AddDays(-1) : source.End.ToUniversalTime();
            }
            // cal.startdate.TimeOfDay == TimeSpan.Zero = source.AllDayEvent; 
            cal.body = source.Body;
            cal.location = source.Location;

            // the rest
            cal.showtimeas = cal.GetShowTimeAs(source.BusyStatus);
            cal.categories = source.Categories;
            // newCal.Duration = calculated by start/end;
            // newCal.Organizer = cal.organizer != null ? cal.organizer.displayname : string.Empty; ReadOnly ???
            // cal.alarm != 0 = source.ReminderSet;
            cal.alarm = source.ReminderSet ? source.ReminderMinutesBeforeStart : 0;
            cal.sensitivity = cal.GetSensitivity(source.Sensitivity);

            // TODO: Recurring
            if (source.IsRecurring)
            {
                var pattern = source.GetRecurrencePattern();

                cal.startdate = source.AllDayEvent ? source.Start.Date : source.Start.ToUniversalTime();
                cal.enddate = cal.startdate.AddMinutes(pattern.Duration);
                if (source.AllDayEvent)
                {
                    cal.enddate = cal.enddate.AddDays(-1);
                }

                if (cal.recurrence == null) cal.recurrence = new OutlookKolab.Kolab.Xml.eventRecurrence();
                cal.recurrence.cycle = cal.GetCycle(pattern.RecurrenceType);
                cal.recurrence.day = cal.GetDay(pattern.DayOfWeekMask);
                if (pattern.RecurrenceType == Outlook.OlRecurrenceType.olRecursYearNth || pattern.RecurrenceType == Outlook.OlRecurrenceType.olRecursMonthNth)
                {
                    cal.recurrence.daynumber = pattern.Instance;
                }
                else
                {
                    cal.recurrence.daynumber = pattern.DayOfMonth;
                }

                if (pattern.RecurrenceType == Outlook.OlRecurrenceType.olRecursYearly || pattern.RecurrenceType == Outlook.OlRecurrenceType.olRecursYearNth)
                {
                    // Ignore Intervals on Years - only Outlook 2007 uses that. 
                    // Outlook < 2007 does not set this property. 
                    // Even worse - Outlook 2007 and 2010 sets the interval value in Months (!) not in years!
                    cal.recurrence.interval = 1;
                }
                else
                {
                    cal.recurrence.interval = pattern.Interval;
                }
                cal.recurrence.month = cal.GetMonth(pattern.MonthOfYear);
                cal.recurrence.range = new OutlookKolab.Kolab.Xml.eventRecurrenceRange();
                cal.recurrence.range.type = cal.GetRangeType(pattern);
                cal.recurrence.range.Value = cal.GetRangeValue(pattern);
                cal.recurrence.type = cal.GetRecurrenceType(pattern);
            }

            return Xml.XmlHelper.ToString(cal);
        }

        /// <summary>
        /// Creates a MailMessage body text
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>MailMessage body text</returns>
        public override string getMessageBodyText(SyncContext sync)
        {
            var cal = getLocalItem(sync);
            StringBuilder sb = new StringBuilder();

            // Default Properties
            sb.AppendLine("Subject: " + cal.Subject);
            sb.AppendLine("Start: " + cal.Start);
            sb.AppendLine("End: " + cal.End);
            sb.AppendLine("AllDayEvent: " + cal.AllDayEvent);
            sb.AppendLine("Body: " + cal.Body);
            sb.AppendLine("Location: " + cal.Location);

            // the rest
            sb.AppendLine("BusyStatus: " + cal.BusyStatus);
            sb.AppendLine("Categories: " + cal.Categories);
            sb.AppendLine("Duration: " + cal.Duration);
            sb.AppendLine("Organizer: " + cal.Organizer);
            sb.AppendLine("ReminderSet: " + cal.ReminderSet);
            sb.AppendLine("ReminderMinutesBeforeStart: " + cal.ReminderMinutesBeforeStart);
            sb.AppendLine("Sensitivity: " + cal.Sensitivity);

            // Recurring
            if (cal.IsRecurring)
            {
                sb.AppendLine("-- IsRecurring --");
                var pattern = cal.GetRecurrencePattern();
                sb.AppendLine("DayOfMonth: " + pattern.DayOfMonth);
                sb.AppendLine("DayOfWeekMask: " + pattern.DayOfWeekMask);
                sb.AppendLine("Duration: " + pattern.Duration);
                sb.AppendLine("EndTime: " + pattern.EndTime);
                sb.AppendLine("Instance: " + pattern.Instance);
                sb.AppendLine("Interval: " + pattern.Interval);
                sb.AppendLine("MonthOfYear: " + pattern.MonthOfYear);
                sb.AppendLine("NoEndDate: " + pattern.NoEndDate);
                sb.AppendLine("Occurrences: " + pattern.Occurrences);
                sb.AppendLine("PatternEndDate: " + pattern.PatternEndDate);
                sb.AppendLine("PatternStartDate: " + pattern.PatternStartDate);
                sb.AppendLine("RecurrenceType: " + pattern.RecurrenceType);
                sb.AppendLine("StartTime: " + pattern.StartTime);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Short text of the current local item
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>short text</returns>
        public override string GetItemText(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            var item = sync.LocalItem as Outlook.AppointmentItem;
            if (item != null)
            {
                return item.Subject + ": " + item.Start.ToString();
            }
            else
            {
                return sync.Message.Subject;
            }
        }
    }
}