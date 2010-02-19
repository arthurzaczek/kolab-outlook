using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookKolab.Kolab
{
    public class Log
    {
        public static void d(string source, string msg)
        {
            System.Diagnostics.Trace.TraceInformation(source + ": " + msg);
        }
        public static void i(string source, string msg)
        {
            System.Diagnostics.Trace.TraceInformation(source + ": " + msg);
        }
        public static void w(string source, string msg)
        {
            System.Diagnostics.Trace.TraceWarning(source + ": " + msg);
        }
        public static void e(string source, string msg)
        {
            System.Diagnostics.Trace.TraceError(source + ": " + msg);
        }
    }
}
