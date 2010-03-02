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

namespace OutlookKolab.Kolab.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Outlook = Microsoft.Office.Interop.Outlook;

    public partial class @event
    {
        /// <summary>
        /// Translates Kolab showtimeas to Oulook busy status
        /// </summary>
        /// <returns>OlBusyStatus, default: olBusy</returns>
        public Outlook.OlBusyStatus GetBusyStatus()
        {
            switch (this.showtimeas)
            {
                case "free": return Outlook.OlBusyStatus.olFree;
                case "tentative": return Outlook.OlBusyStatus.olTentative;
                case "busy": return Outlook.OlBusyStatus.olBusy;
                case "outofoffice": return Outlook.OlBusyStatus.olOutOfOffice;
                default: return Outlook.OlBusyStatus.olBusy;
            }
        }

        /// <summary>
        /// Translates Outlook busy status to Kolab showtimeas
        /// </summary>
        /// <param name="status">OlBusyStatus</param>
        /// <returns>showtimeas, default: busy</returns>
        public string GetShowTimeAs(Outlook.OlBusyStatus status)
        {
            switch (status)
            {
                case Outlook.OlBusyStatus.olBusy: return "busy";
                case Outlook.OlBusyStatus.olFree: return "free";
                case Outlook.OlBusyStatus.olOutOfOffice: return "outofoffice";
                case Outlook.OlBusyStatus.olTentative: return "tentative";
                default: return "busy";
            }
        }

        /// <summary>
        /// Translates Kolab sensitivity to Outlook sensitivity
        /// </summary>
        /// <returns>OlSensitivity, default: olNormal</returns>
        public Outlook.OlSensitivity GetSensitivity()
        {
            switch (this.sensitivity)
            {
                case "private": return Outlook.OlSensitivity.olPrivate;
                case "confidential ": return Outlook.OlSensitivity.olConfidential;
                case "public": return Outlook.OlSensitivity.olNormal;
                default: return Outlook.OlSensitivity.olNormal;
            }
        }

        /// <summary>
        /// Translates Outlook sensitivity to Kolab sensitivity
        /// </summary>
        /// <param name="s">OlSensitivity</param>
        /// <returns>sensitivity, default: public</returns>
        public string GetSensitivity(Outlook.OlSensitivity s)
        {
            switch (s)
            {
                case Outlook.OlSensitivity.olPrivate: return "private";
                case Outlook.OlSensitivity.olPersonal: return "private";
                case Outlook.OlSensitivity.olConfidential: return "confidential";
                case Outlook.OlSensitivity.olNormal: return "public";
                default: return "public";
            }
        }

        /// <summary>
        /// Translates Outlook recurrence type to Kolab cycle type
        /// </summary>
        /// <param name="type">OlRecurrenceType</param>
        /// <returns>cycle type, default: ""</returns>
        public string GetCycle(Outlook.OlRecurrenceType type)
        {
            switch (type)
            {
                case Outlook.OlRecurrenceType.olRecursDaily: return "daily";
                case Outlook.OlRecurrenceType.olRecursWeekly: return "weekly";
                case Outlook.OlRecurrenceType.olRecursMonthly: return "monthly";
                case Outlook.OlRecurrenceType.olRecursMonthNth: return "monthly";
                case Outlook.OlRecurrenceType.olRecursYearly: return "yearly";
                case Outlook.OlRecurrenceType.olRecursYearNth: return "yearly";
                default: return "";
            }
        }

        /// <summary>
        /// Translates Kolab cycle type to Outlook recurrence type
        /// </summary>
        /// <returns>OlRecurrenceType, default: olRecursDaily</returns>
        public Outlook.OlRecurrenceType GetRecurrenceType()
        {
            switch (this.recurrence.cycle)
            {
                case "daily": return Outlook.OlRecurrenceType.olRecursDaily;
                case "weekly": return Outlook.OlRecurrenceType.olRecursWeekly;
                case "monthly": return this.recurrence.day == null ? Outlook.OlRecurrenceType.olRecursMonthly : Outlook.OlRecurrenceType.olRecursMonthNth;
                case "yearly": return this.recurrence.day == null ? Outlook.OlRecurrenceType.olRecursYearly : Outlook.OlRecurrenceType.olRecursYearNth;
                default: return Outlook.OlRecurrenceType.olRecursDaily;
            }
        }

        /// <summary>
        /// Translates Outlook DaysOfWeek to Kolab days collection
        /// </summary>
        /// <param name="days">OlDaysOfWeek</param>
        /// <returns>collection of days, default: null</returns>
        public string[] GetDay(Outlook.OlDaysOfWeek days)
        {
            List<string> result = new List<string>();
            if ((days & Outlook.OlDaysOfWeek.olMonday) != 0)
            {
                result.Add("monday");
            }
            if ((days & Outlook.OlDaysOfWeek.olTuesday) != 0)
            {
                result.Add("tuesday");
            }
            if ((days & Outlook.OlDaysOfWeek.olWednesday) != 0)
            {
                result.Add("wednesday");
            }
            if ((days & Outlook.OlDaysOfWeek.olThursday) != 0)
            {
                result.Add("thursday");
            }
            if ((days & Outlook.OlDaysOfWeek.olFriday) != 0)
            {
                result.Add("friday");
            }
            if ((days & Outlook.OlDaysOfWeek.olSaturday) != 0)
            {
                result.Add("saturday");
            }
            if ((days & Outlook.OlDaysOfWeek.olSunday) != 0)
            {
                result.Add("sunday");
            }

            return result.Count > 0 ? result.ToArray() : null;
        }

        /// <summary>
        /// Translates Kolab days collection to Outlook DaysOfWeek
        /// </summary>
        /// <returns>OlDaysOfWeek, default: olMonday</returns>
        public Outlook.OlDaysOfWeek GetDayOfWeekMask()
        {
            Outlook.OlDaysOfWeek result = 0;
            if (this.recurrence.day == null) return Microsoft.Office.Interop.Outlook.OlDaysOfWeek.olMonday;
            foreach (string day in this.recurrence.day)
            {
                switch (day)
                {
                    case "monday": result |= Outlook.OlDaysOfWeek.olMonday; break;
                    case "tuesday": result |= Outlook.OlDaysOfWeek.olTuesday; break;
                    case "wednesday": result |= Outlook.OlDaysOfWeek.olWednesday; break;
                    case "thursday": result |= Outlook.OlDaysOfWeek.olThursday; break;
                    case "friday": result |= Outlook.OlDaysOfWeek.olFriday; break;
                    case "saturday": result |= Outlook.OlDaysOfWeek.olSaturday; break;
                    case "sunday": result |= Outlook.OlDaysOfWeek.olSunday; break;
                }
            }
            return result;
        }

        public static List<string> months = new List<string>() { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };

        /// <summary>
        /// Transaltes a month number (1-12) to Kolab month string
        /// </summary>
        /// <param name="month">month number (1-12)</param>
        /// <returns>month as string</returns>
        public string GetMonth(int month)
        {
            if (month >= 1 && month <= 12)
            {
                return months[month - 1];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Translates a Kolab month string to a month number (1-12)
        /// </summary>
        /// <returns>month number (1-12)</returns>
        public int GetMonthOfYear()
        {
            return months.IndexOf(recurrence.month) + 1;
        }

        /// <summary>
        /// Translates a Outlook recurrence pattern to Kolab range type
        /// </summary>
        /// <param name="pattern">RecurrencePattern</param>
        /// <returns>range type as string</returns>
        public string GetRangeType(Outlook.RecurrencePattern pattern)
        {
            if (pattern == null) { throw new ArgumentNullException("pattern"); }

            if (pattern.NoEndDate)
            {
                return "none";
            }
            else if (pattern.PatternEndDate.IsValid())
            {
                return "date";
            }
            else if (pattern.Occurrences != 0)
            {
                return "number";
            }

            return "none";
        }

        /// <summary>
        /// Translates a Outlook recurrence pattern to Kolab range value
        /// </summary>
        /// <param name="pattern">RecurrencePattern</param>
        /// <returns>range value as string</returns>
        public string GetRangeValue(Outlook.RecurrencePattern pattern)
        {
            if (pattern == null) { throw new ArgumentNullException("pattern"); }

            if (!pattern.NoEndDate) return pattern.PatternEndDate.ToString("u");
            if (pattern.Occurrences != 0) return pattern.Occurrences.ToString();
            return "";
        }

        /// <summary>
        /// Translates a Outlook recurrence pattern to Kolab cycle type
        /// </summary>
        /// <param name="pattern">RecurrencePattern</param>
        /// <returns>cycle type</returns>
        public string GetRecurrenceType(Microsoft.Office.Interop.Outlook.RecurrencePattern pattern)
        {
            if (pattern == null) { throw new ArgumentNullException("pattern"); }

            if (pattern.RecurrenceType == Outlook.OlRecurrenceType.olRecursMonthly)
            {
                if (pattern.DayOfMonth != 0) return "daynumber";
                return "weekday";
            }

            if (pattern.RecurrenceType == Outlook.OlRecurrenceType.olRecursYearly)
            {
                if (pattern.MonthOfYear != 0 && pattern.DayOfMonth != 0) return "monthday";
                if (pattern.DayOfWeekMask != 0) return "weekday";
            }

            return "";
        }
    }
}
