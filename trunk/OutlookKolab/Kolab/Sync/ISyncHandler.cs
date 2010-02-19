using System;
using System.Collections;
using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;
using OutlookKolab.Kolab.Provider;

namespace OutlookKolab.Kolab.Sync
{

    public interface ISyncHandler
    {
        DSStatus.StatusEntryRow getStatus();

        IEnumerable<string> getAllLocalItemIDs();

        String GetIMAPFolderName();
        String GetIMAPStoreID();

        LocalCacheProvider getLocalCacheProvider();

        bool hasLocalItem(SyncContext sync);
        bool hasLocalChanges(SyncContext sync);

        void createLocalItemFromServer(SyncContext sync);
        void createServerItemFromLocal(Outlook.Folder targetFolder, SyncContext sync, string localId);

        void updateLocalItemFromServer(SyncContext sync);
        void updateServerItemFromLocal(Outlook.Folder targetFolder, SyncContext sync);

        void deleteLocalItem(SyncContext sync);
        void deleteServerItem(SyncContext sync);
    }
}