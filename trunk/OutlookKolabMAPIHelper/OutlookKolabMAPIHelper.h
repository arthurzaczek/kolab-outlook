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

// OutlookKolabMAPIHelper.h

#pragma once

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;

#define HIDOUBLEWORD(x)    ((x>>32) & 0xffffffff)
#define LODOUBLEWORD(x)    ((x)     & 0xffffffff)

// Outlook managed C++ MAPI Helper
// Contains static helper methods for getting/setting informations which Outlooks Object Model does not provide
namespace OutlookKolabMAPIHelper {

	public ref class IMAPHelper
	{
	public:
		// Sets the SentDate on an Outlook IMessage object
		// System::IntPtr^ outlookObject: Outlook IMessage MAPI object
		// DateTime^ dt: DateTime to set
		static void SetSentDate(System::IntPtr^ outlookObject, DateTime^ dt)
		{
			// Get IUnknown from IntPtr
			IUnknown* iUnkn = static_cast<IUnknown*>(outlookObject->ToPointer());
			LPMAPIPROP prop = 0;
			HRESULT hr;
			// Query for IMAPIProp
			if(hr = iUnkn->QueryInterface(IID_IMAPIProp, (void**)&prop))
			{
				throw gcnew InvalidOperationException(System::String::Format("Unable to query for IMAPIProp Interface: hr = 0x{0:X8}", hr));
			}

			try
			{
				// Construct Property Value
				SPropValue value;
				memset(&value, 0, sizeof(SPropValue));
				// Property: SentDate
				value.ulPropTag = PR_CLIENT_SUBMIT_TIME;
				// Convert DateTime to FileTime (SysTime in MAPI)
				__int64 ft = dt->ToFileTimeUtc();
				value.Value.ft.dwHighDateTime = HIDOUBLEWORD(ft);
				value.Value.ft.dwLowDateTime = LODOUBLEWORD(ft);
				
				// Set Property
				if(hr = prop->SetProps(1, &value, 0))
				{
					throw gcnew InvalidOperationException(System::String::Format("Unable to set PR_CLIENT_SUBMIT_TIME Property: hr = 0x{0:X8}", hr));
				}
			}
			finally
			{
				prop->Release();
			}
		}

		// Reads and Outlook Attachment and returnes it content as string
		// System::IntPtr^ outlookAttachment: Outlook IAttachment MAPI Object
		// returnes: content as string
		static System::String^ ReadAttachment(System::IntPtr^ outlookAttachment)
		{
			// Get IUnknown from IntPtr
			IUnknown* iUnkn = static_cast<IUnknown*>(outlookAttachment->ToPointer());
			LPATTACH a = 0;
			HRESULT hr;
			// Query for IAttachment
			if(hr = iUnkn->QueryInterface(IID_IAttachment, (void**)&a))
			{
				throw gcnew InvalidOperationException(System::String::Format("Unable to query for IAttachment Interface: hr = 0x{0:X8}", hr));
			}

			System::String^ result = "";
			LPSTREAM stream = 0;
			try
			{
				// Get Content property
				if (hr = a->OpenProperty(PR_ATTACH_DATA_BIN, (LPIID)&IID_IStream, 0, MAPI_MODIFY, (LPUNKNOWN*)&stream))
				{
					throw gcnew InvalidOperationException(System::String::Format("Unable to get PR_ATTACH_DATA_BIN Property: hr = 0x{0:X8}", hr));
				}
				// Get Attachment infos
				STATSTG statInfo;
				stream->Stat(&statInfo, STATFLAG_NONAME);
				// dont read large attachments
				if(statInfo.cbSize.HighPart == 0) 
				{
					ULONG size = statInfo.cbSize.LowPart;
					// Alloc buffer
					unsigned char* buffer = (unsigned char*)malloc(size);
					// Read into buffer
					ULONG readBytes;
					stream->Read(buffer, size, &readBytes);
					Debug::Assert(size == readBytes);
					// Convert to managed string
					System::IO::UnmanagedMemoryStream^ ms = gcnew System::IO::UnmanagedMemoryStream(buffer, size);
					System::IO::StreamReader^ sr = gcnew System::IO::StreamReader(ms, System::Text::Encoding::UTF8);
					result = sr->ReadToEnd();
					// Free buffer
					free(buffer);
				}
				else
				{
					throw gcnew InvalidOperationException("Attachment too large");
				}
			}
			finally
			{
				if(stream) stream->Release();
				if(a) a->Release();
			}

			return result;
		}

		// Returnes a List of Entry IDs of deleted IMAP Messages
		// System::IntPtr^ outlookFolder: Outlook IMAPIFolder MAPI Object
		// returnes: List of deleted deleted IMAP Messages as List<String>
		static List<System::String^>^ GetDeletedEntryIDs(System::IntPtr^ outlookFolder)
		{
		
			// Get IUnknown from IntPtr
			IUnknown* iUnkn = static_cast<IUnknown*>(outlookFolder->ToPointer());
			IMAPIFolder* fld;
			HRESULT hr;
			// Query for IMAPIFolder
			if(hr = iUnkn->QueryInterface(IID_IMAPIFolder, (void**)&fld))
			{
				throw gcnew InvalidOperationException(System::String::Format("Unable to query for IMAPIFolder Interface: hr = 0x{0:X8}", hr));
			}

			List<System::String^>^ result = gcnew List<System::String^>();
			try
			{
				// Get ContentsTable
				LPMAPITABLE tbl;
				if(hr = fld->GetContentsTable(0, &tbl))
				{
					throw gcnew InvalidOperationException(System::String::Format("Unable to get content table: hr = 0x{0:X8}", hr));
				}
				else
				{
					// Loop until QueryRows returns 0 rows
					while(1)
					{
						// Query for rows
						LPSRowSet rows;
						tbl->QueryRows(1024, 0, &rows);
						if(rows->cRows == 0) 
						{
							// No more rows - break
							MAPIFreeBuffer(rows);
							break;
						}

						// foreach row
						for(UINT i=0;i<rows->cRows;i++)
						{
							// Read row
							SRow r = rows->aRow[i];
							long outlook_flag = 0;
							SBinary outlook_entryid;
							outlook_entryid.cb = 0;

							// Search for interesting values (Message Flags, ENTRY_ID)
							for(UINT c=0;c<r.cValues;c++)
							{
								UINT tag = HIWORD(r.lpProps[c].ulPropTag);
								_PV val = r.lpProps[c].Value;

								// Message Flags
								if(tag == 0x8019) 
								{
									outlook_flag = val.l;
								}
								// ENTRY_ID
								if(tag == 0x0FFF) 
								{
									outlook_entryid = val.bin;
								}
							}

							// If found both and bit 4 is set -> save in list
							// Bit 4 seams to be the marker for deleted messages
							if(outlook_entryid.cb && (outlook_flag & 4))
							{
								// Convert EntryID to a String
								System::Text::StringBuilder^ sb = gcnew System::Text::StringBuilder();
								for(unsigned int i=0;i<outlook_entryid.cb;i++)
								{
									sb->AppendFormat("{0:X2}", outlook_entryid.lpb[i]);
								}
								// Add to result
								result->Add(sb->ToString());
							}

						}
						// Free row
						MAPIFreeBuffer(rows);
					}
					// Release table
					tbl->Release();
				}
			}
			finally
			{
				// Release folder
				fld->Release();
			}

			return result;
		}
	};
}
