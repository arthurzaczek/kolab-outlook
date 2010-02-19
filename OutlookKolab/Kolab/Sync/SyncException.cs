using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookKolab.Kolab.Sync
{
    public class SyncException : Exception
    {
        public SyncException(string item, string message)
            : base(message)
        {
            this.Item = item;
        }

        public SyncException(string item, string message, Exception inner)
            : base(message, inner)
        {
            this.Item = item;
        }

        public string Item { get; private set; }
    }
}
