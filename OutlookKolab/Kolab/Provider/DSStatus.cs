using System.IO;
using OutlookKolab.Kolab.Sync;
namespace OutlookKolab.Kolab.Provider {
    
    
    public partial class DSStatus {

        public static DSStatus Load()
        {
            DSStatus status = new DSStatus();
            status.ReLoad();
            return status;
        }

        public void ReLoad()
        {
            if (File.Exists(Helper.StatusPath))
            {
                this.ReadXml(Helper.StatusPath);
            }
        }

        public void Save()
        {
            Helper.EnsureStorePath();
            this.WriteXml(Helper.StatusPath);
        }

        partial class StatusEntryRow
        {
            public int incrementItems()
            {
                return ++items;
            }

            public int incrementLocalChanged()
            {
                return ++localChanged;
            }

            public int incrementRemoteChanged()
            {
                return ++remoteChanged;
            }

            public int incrementLocalNew()
            {
                return ++localNew;
            }

            public int incrementRemoteNew()
            {
                return ++remoteNew;
            }

            public int incrementLocalDeleted()
            {
                return ++localDeleted;
            }

            public int incrementRemoteDeleted()
            {
                return ++remoteDeleted;
            }

            public int incrementConflicted()
            {
                return ++conflicted;
            }

            public int incrementErrors(SyncException ex)
            {
                DSStatus ds = (DSStatus)this.Table.DataSet;
                ds.Error.AddErrorRow(this, ex.Message, ex.ToString(), ex.Item);
                return ++errors;
            }
        }
    }
}
