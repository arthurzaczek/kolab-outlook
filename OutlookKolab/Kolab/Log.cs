﻿/*
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

namespace OutlookKolab.Kolab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    /// <summary>
    /// Logging Helper class.
    /// Wrapps Logging support
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="source">logging source</param>
        /// <param name="msg">message to log</param>
        public static void d(string source, string msg)
        {
            System.Diagnostics.Trace.WriteLine(source + ": " + msg);
        }
        /// <summary>
        /// Logs a information message
        /// </summary>
        /// <param name="source">logging source</param>
        /// <param name="msg">message to log</param>
        public static void i(string source, string msg)
        {
            System.Diagnostics.Trace.TraceInformation(source + ": " + msg);
        }
        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="source">logging source</param>
        /// <param name="msg">message to log</param>
        public static void w(string source, string msg)
        {
            System.Diagnostics.Trace.TraceWarning(source + ": " + msg);
        }
        /// <summary>
        /// Logs a error message
        /// </summary>
        /// <param name="source">logging source</param>
        /// <param name="msg">message to log</param>
        public static void e(string source, string msg)
        {
            System.Diagnostics.Trace.TraceError(source + ": " + msg);
        }
    }
}
