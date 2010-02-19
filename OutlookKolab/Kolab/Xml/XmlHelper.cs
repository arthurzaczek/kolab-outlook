using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace OutlookKolab.Kolab.Xml
{
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
