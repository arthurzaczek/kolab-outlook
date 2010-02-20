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
    using System.Collections;
    using System.Collections.Generic;

    using OutlookKolab.Kolab.Provider;
    using Outlook = Microsoft.Office.Interop.Outlook;

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