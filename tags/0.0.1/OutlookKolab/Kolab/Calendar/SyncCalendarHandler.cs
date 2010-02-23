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

    public class SyncCalendarHandler : AbstractSyncHandler
    {
        LocalCacheProvider cache = null;

        public SyncCalendarHandler(DSSettings settings, DSStatus dsStatus, Outlook.Application app)
            : base(settings, dsStatus, app)
        {
            cache = new LocalCacheProvider(LocalCacheProviderType.Calendar);
            status.task = "Calendar";
        }

        public override IEnumerable<string> getAllLocalItemIDs()
        {
            return Folder.Items.OfType<Outlook.AppointmentItem>().Select(i => i.EntryID);
        }

        public override string GetIMAPFolderName()
        {
            return settings.Settings[0].CalendarIMAPFolder;
        }
        public override string GetIMAPStoreID()
        {
            return settings.Settings[0].CalendarIMAPStore;
        }

        public override string GetOutlookFolderName()
        {
            return settings.Settings[0].CalendarOutlookFolder;
        }
        public override string GetOutlookStoreID()
        {
            return settings.Settings[0].CalendarOutlookStore;
        }

        public override LocalCacheProvider getLocalCacheProvider()
        {
            return cache;
        }

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

        public override bool hasLocalChanges(SyncContext sync)
        {
            if (sync == null) { throw new ArgumentNullException("sync"); }

            Log.i("sync", "Checking for local changes: #" + sync.CacheEntry.localId);
            var cal = getLocalItem(sync);
            String entryHash = sync.CacheEntry.localHash;
            String contactHash = cal != null ? getLocalHash(cal) : "";
            return entryHash != contactHash;
        }

        public override bool hasLocalItem(SyncContext sync)
        {
            return getLocalItem(sync) != null;
        }

        protected override string updateServerItemFromLocal(SyncContext sync, string xml)
        {
            var source = getLocalItem(sync);
            sync.CacheEntry.localHash = getLocalHash(source);
            sync.CacheEntry.remoteChangedDate = DateTime.Now.ToUniversalTime();

            var cal = Xml.XmlHelper.ParseCalendar(xml);
            return writeXml(source, cal, sync.CacheEntry.remoteChangedDate);
        }

        protected override void deleteLocalItem(string localId)
        {
            var e = app.Session.GetItemFromID(localId, Folder.StoreID) as Outlook.AppointmentItem;
            if (e != null) e.Delete();
        }

        protected override string getMimeType()
        {
            return "application/x-vnd.kolab.event";
        }

        protected override void updateLocalItemFromServer(SyncContext sync, string xml)
        {
            Xml.@event cal = null;
            try
            {
                cal = Xml.XmlHelper.ParseCalendar(xml);
            }
            catch (Exception ex)
            {
                throw new SyncException(GetItemText(sync), "Unable to parse XML Document", ex);
            }

            var localCal = (Outlook.AppointmentItem)sync.LocalItem;
            if (localCal == null)
            {
                localCal = (Outlook.AppointmentItem)Folder.Items.Add(Outlook.OlItemType.olAppointmentItem);
            }

            bool isRecurring = cal.recurrence != null && !string.IsNullOrEmpty(cal.recurrence.cycle);

            try
            {
                // Android part
                localCal.Subject = cal.summary;
                if (!isRecurring)
                {
                    localCal.StartUTC = cal.startdate.ToUniversalTime();
                    localCal.EndUTC = cal.enddate.ToUniversalTime();
                    localCal.AllDayEvent = cal.startdate.TimeOfDay == TimeSpan.Zero;
                }
                localCal.Body = cal.body;
                localCal.Location = cal.location;

                // the rest
                localCal.BusyStatus = cal.GetBusyStatus();
                localCal.Categories = cal.categories;
                // localCal.Duration = calculated by start/end;
                // localCal.Organizer = cal.organizer != null ? cal.organizer.displayname : string.Empty; ReadOnly ???
                localCal.ReminderSet = cal.alarm != 0;
                localCal.ReminderMinutesBeforeStart = cal.alarm;
                localCal.Sensitivity = cal.GetSensitivity();
            }
            catch (COMException ex)
            {
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

                    DateTime endDate = cal.enddate;
                    if (endDate.Kind == DateTimeKind.Utc) endDate = endDate.ToLocalTime();
                    var endTime = startDate.TimeOfDay;

                    var duration = endDate - startDate;

                    pattern.StartTime = DateTime.MinValue + startTime;
                    pattern.EndTime = DateTime.MinValue + endTime;
                    pattern.Duration = (int)duration.TotalMinutes; 

                    // Only if valid or not yearly - only outlook 2007 does support yearly recurrences
                    if (cal.recurrence.interval != 0 &&
                        pattern.RecurrenceType != Microsoft.Office.Interop.Outlook.OlRecurrenceType.olRecursYearly &&
                        pattern.RecurrenceType != Microsoft.Office.Interop.Outlook.OlRecurrenceType.olRecursYearNth)
                    {
                        pattern.Interval = cal.recurrence.interval;
                    }
                    else
                    {
                        pattern.Interval = 1;
                    }

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
                    throw new SyncException(GetItemText(sync), "Unable to set AppointmentItem recurrence", ex);
                }
            }
            else
            {
                localCal.ClearRecurrencePattern();
            }

            try
            {
                localCal.Save();
            }
            catch (COMException ex)
            {
                throw new SyncException(GetItemText(sync), "Unable to save AppointmentItem", ex);
            }

            if (sync.CacheEntry == null)
            {
                sync.CacheEntry = getLocalCacheProvider().createEntry();
            }
            sync.CacheEntry.localId = localCal.EntryID;
            sync.CacheEntry.localHash = getLocalHash(localCal);

        }

        private string getNewUid()
        {
            // Create Application and Type specific id
            // kd == Kolab Outlook
            return "ko-ev-" + Guid.NewGuid().ToString();
        }

        protected override string writeXml(SyncContext sync)
        {
            var item = getLocalItem(sync);
            sync.CacheEntry.localHash = getLocalHash(item);
            sync.CacheEntry.remoteChangedDate = DateTime.Now.ToUniversalTime();
            sync.CacheEntry.remoteId = getNewUid();
            return writeXml(item, new OutlookKolab.Kolab.Xml.@event(), sync.CacheEntry.remoteChangedDate);
        }

        private string writeXml(Microsoft.Office.Interop.Outlook.AppointmentItem source, OutlookKolab.Kolab.Xml.@event cal, DateTime lastmodificationdate)
        {
            cal.lastmodificationdate = lastmodificationdate;
            cal.summary = source.Subject;
            // StartUTC/EndUTC does not specify DateTime.Kind == Utc!
            cal.startdate = source.AllDayEvent ? source.Start.Date : source.Start.ToUniversalTime();
            cal.enddate = source.AllDayEvent ? source.End.Date : source.End.ToUniversalTime();
            // cal.startdate.TimeOfDay == TimeSpan.Zero = source.AllDayEvent; 
            cal.body = source.Body;
            cal.location = source.Location;

            // the rest
            cal.showtimeas = cal.GetShowTimeAs(source.BusyStatus);
            cal.categories = source.Categories;
            // newCal.Duration = calculated by start/end;
            // newCal.Organizer = cal.organizer != null ? cal.organizer.displayname : string.Empty; ReadOnly ???
            // cal.alarm != 0 = source.ReminderSet;
            cal.alarm = source.ReminderMinutesBeforeStart;
            cal.sensitivity = cal.GetSensitivity(source.Sensitivity);

            // TODO: Recurring
            if (source.IsRecurring)
            {
                var pattern = source.GetRecurrencePattern();
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
                    // Even worse - Outlook 2007 sets the interval value in Months (!) not in years!
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

        public override string getMessageBodyText(SyncContext sync)
        {
            var cal = getLocalItem(sync);
            StringBuilder sb = new StringBuilder();

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