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

    /// <summary>
    /// UTF8 String Writer
    /// </summary>
    public sealed class Utf8StringWriter : StringWriter
    {
        /// <summary>
        /// Overrides the encoding
        /// </summary>
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }

    /// <summary>
    /// XML Helper
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Contact serializer
        /// </summary>
        static XmlSerializer contactSer = new XmlSerializer(typeof(contact));
        /// <summary>
        /// Event/Calendar serializer
        /// </summary>
        static XmlSerializer calendarSer = new XmlSerializer(typeof(@event));

        /// <summary>
        /// Parses a Kolab Contact XML
        /// </summary>
        /// <param name="xml">Kolab xml</param>
        /// <returns>Kolab contact object</returns>
        public static contact ParseContact(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                return (contact)contactSer.Deserialize(sr);
            }
        }

        /// <summary>
        /// Parses a Kolab Calendar XML
        /// </summary>
        /// <param name="xml">Kolab xml</param>
        /// <returns>Kolab event/calendar object</returns>
        public static @event ParseCalendar(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                return (@event)calendarSer.Deserialize(sr);
            }
        }

        /// <summary>
        /// Serializes a Kolab Contact XML Object to XML String
        /// </summary>
        /// <param name="contact">Kolab Contact XML Object</param>
        /// <returns>XML String</returns>
        public static string ToString(contact contact)
        {
            using (var sw = new Utf8StringWriter())
            {
                contactSer.Serialize(sw, contact);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Serializes a Kolab Calendar/Event XML Object to XML String
        /// </summary>
        /// <param name="contact">Kolab Calendar/Event XML Object</param>
        /// <returns>XML String</returns>
        public static string ToString(@event contact)
        {
            using (var sw = new Utf8StringWriter())
            {
                calendarSer.Serialize(sw, contact);
                return sw.ToString();
            }
        }
    }
}
