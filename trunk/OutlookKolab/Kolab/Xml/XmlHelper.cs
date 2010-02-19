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
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }

    public class XmlHelper
    {
        static XmlSerializer contactSer = new XmlSerializer(typeof(contact));
        static XmlSerializer calendarSer = new XmlSerializer(typeof(@event));

        public static contact ParseContact(string xml)
        {
            return (contact)contactSer.Deserialize(new StringReader(xml));
        }

        public static @event ParseCalendar(string xml)
        {
            return (@event)calendarSer.Deserialize(new StringReader(xml));
        }

        public static string ToString(contact contact)
        {
            Utf8StringWriter sw = new Utf8StringWriter();
            contactSer.Serialize(sw, contact);
            return sw.ToString();
        }

        public static string ToString(@event contact)
        {
            Utf8StringWriter sw = new Utf8StringWriter();
            calendarSer.Serialize(sw, contact);
            return sw.ToString();
        }
    }
}
