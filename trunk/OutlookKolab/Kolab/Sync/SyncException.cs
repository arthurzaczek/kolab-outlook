using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookKolab.Kolab.Sync
{
    public class SyncException : Exception
    {
        public SyncException()
        {
        }

        public SyncException(string message)
            : base(message)
        {
        }

        public SyncException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
