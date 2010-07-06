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


namespace OutlookKolab.Kolab.Provider
{
    using System.IO;

    using OutlookKolab.Kolab.Sync;

    public partial class DSStatus
    {

        public static DSStatus Load()
        {
            DSStatus status = new DSStatus();
            status.ReLoad();
            return status;
        }

        public void ReLoad()
        {
            FileTransaction.FixBrokenTransaction(Helper.StatusPath);
            if (File.Exists(Helper.StatusPath))
            {
                this.ReadXml(Helper.StatusPath);
            }
        }

        public void Save()
        {
            Helper.EnsureStorePath();
            using (var tx = new FileTransaction(Helper.StatusPath))
            {
                this.WriteXml(tx.FullTempFileName);
                tx.Commit();
            }
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
