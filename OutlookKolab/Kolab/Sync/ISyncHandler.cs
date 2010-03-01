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

    /// <summary>
    /// Interface represanting a SyncHandler
    /// </summary>
    public interface ISyncHandler
    {
        /// <summary>
        /// Returns the current status row
        /// </summary>
        /// <returns>DSStatus.StatusEntryRow</returns>
        DSStatus.StatusEntryRow getStatus();

        /// <summary>
        /// Returns all Entry IDs of all local items
        /// </summary>
        /// <returns>List of Entry IDs</returns>
        IEnumerable<string> getAllLocalItemIDs();

        /// <summary>
        /// Current handlers IMAP Folder Entry ID = Remote Items
        /// </summary>
        /// <returns>Entry ID</returns>
        String GetIMAPFolderName();
        /// <summary>
        /// Current handlers IMAP Folder Store ID = Remote Items
        /// </summary>
        /// <returns>Store ID</returns>
        String GetIMAPStoreID();
        /// <summary>
        /// Current handlers local Folder Entry ID = Local Items
        /// </summary>
        /// <returns>Entry ID</returns>
        String GetOutlookFolderName();
        /// <summary>
        /// Current handlers local Folder Store ID = Local Items
        /// </summary>
        /// <returns>Store ID</returns>
        String GetOutlookStoreID();

        /// <summary>
        /// Returns the local cache provider of the current handler
        /// </summary>
        /// <returns>LocalCacheProvider</returns>
        LocalCacheProvider getLocalCacheProvider();

        /// <summary>
        /// checks if the local item exits
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>true if the local item exits</returns>
        bool hasLocalItem(SyncContext sync);
        /// <summary>
        /// checks for local changes
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>true if the local item changes since last sync</returns>
        bool hasLocalChanges(SyncContext sync);

        /// <summary>
        /// create a local item from the given server items
        /// </summary>
        /// <param name="sync">current sync context</param>
        void createLocalItemFromServer(SyncContext sync);
        /// <summary>
        /// Create a new server item from the given local item.
        /// This method creates also a new local cache entry.
        /// </summary>
        /// <param name="imapFolder">destination IMAP Folder</param>
        /// <param name="sync">current sync context</param>
        /// <param name="localId">Entry ID of the local item</param>
        void createServerItemFromLocal(Outlook.Folder imapFolder, SyncContext sync, string localId);

        /// <summary>
        /// update a local item from the given server items
        /// </summary>
        /// <param name="sync">current sync context</param>
        void updateLocalItemFromServer(SyncContext sync);
        /// <summary>
        /// Update a server item from the given local item.
        /// </summary>
        /// <param name="imapFolder">destination IMAP Folder</param>
        /// <param name="sync">current sync context</param>
        void updateServerItemFromLocal(Outlook.Folder imapFolder, SyncContext sync);

        /// <summary>
        /// Deletes the given local item
        /// </summary>
        /// <param name="sync">current sync context</param>
        void deleteLocalItem(SyncContext sync);
        /// <summary>
        /// Deletes the given server item
        /// </summary>
        /// <param name="sync">current sync context</param>
        void deleteServerItem(SyncContext sync);

        /// <summary>
        /// Short text of the current local item
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>short text</returns>
        string GetItemText(SyncContext sync);
        /// <summary>
        /// Creates a MailMessage body text
        /// </summary>
        /// <param name="sync">current sync context</param>
        /// <returns>MailMessage body text</returns>
        string getMessageBodyText(SyncContext sync);
    }
}