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

namespace OutlookKolab.Kolab.Sync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Sync Exception. This exception counts error items and terminate the sync only for the current item. 
    /// It does not terminate the sync process.
    /// </summary>
    [Serializable]
    public class SyncException : Exception
    {
        /// <summary>
        /// Creates a empty SyncException
        /// </summary>
        public SyncException()
        {
        }

        /// <summary>
        /// Creates a SyncException
        /// </summary>
        /// <param name="item">Short descriptive text of the item affected</param>
        /// <param name="message">Exception message</param>
        public SyncException(string item, string message)
            : base(message)
        {
            this.Item = item;
        }

        /// <summary>
        /// Creates a SyncException
        /// </summary>
        /// <param name="item">Short descriptive text of the item affected</param>
        /// <param name="message">Exception message</param>
        /// <param name="inner">inner exception</param>
        public SyncException(string item, string message, Exception inner)
            : base(message, inner)
        {
            this.Item = item;
        }

        /// <summary>
        /// Creates a SyncException if deserialized
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SyncException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Short descriptive text of the item affected
        /// </summary>
        public string Item { get; private set; }
    }
}
