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

// OutlookKolapMAPIHelper.h

#pragma once

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

#define HIDOUBLEWORD(x)    ((x>>32) & 0xffffffff)
#define LODOUBLEWORD(x)    ((x)     & 0xffffffff)

namespace OutlookKolapMAPIHelper {

	public ref class IMAPHelper
	{
	public:
		static void SetSentDate(System::IntPtr^ outlookObject, DateTime^ dt)
		{
			IUnknown* iUnkn = static_cast<IUnknown*>(outlookObject->ToPointer());
			LPMAPIPROP prop = 0;
			HRESULT hr;
			if(hr = iUnkn->QueryInterface(IID_IMAPIProp, (void**)&prop))
			{
				return;
			}

			SPropValue value;
			memset(&value, 0, sizeof(SPropValue));
			value.ulPropTag = PR_CLIENT_SUBMIT_TIME;
			__int64 ft = dt->ToFileTimeUtc();
			value.Value.ft.dwHighDateTime = HIDOUBLEWORD(ft);
			value.Value.ft.dwLowDateTime = LODOUBLEWORD(ft);
			
			hr = prop->SetProps(1, &value, 0);

			prop->Release();
		}

		static System::String^ ReadAttachment(System::IntPtr^ outlookAttachment)
		{
			System::String^ result = "";

			IUnknown* iUnkn = static_cast<IUnknown*>(outlookAttachment->ToPointer());
			LPATTACH a = 0;
			HRESULT hr;
			if(hr = iUnkn->QueryInterface(IID_IAttachment, (void**)&a))
			{
				return result;
			}

			LPSTREAM stream = 0;
			if (SUCCEEDED(hr = a->OpenProperty(PR_ATTACH_DATA_BIN, (LPIID)&IID_IStream, 0, MAPI_MODIFY, (LPUNKNOWN*)&stream)))
			{
				STATSTG statInfo;
				stream->Stat(&statInfo, STATFLAG_NONAME);
				if(statInfo.cbSize.HighPart == 0) // dont read large attachments
				{
					ULONG size = statInfo.cbSize.LowPart;
					unsigned char* buffer = (unsigned char*)malloc(size + 1);
					memset(buffer, 0, size + 1);
					ULONG readBytes;
					stream->Read(buffer, size, &readBytes);
					System::IO::UnmanagedMemoryStream^ ms = gcnew System::IO::UnmanagedMemoryStream(buffer, size + 1);
					System::IO::StreamReader^ sr = gcnew System::IO::StreamReader(ms, System::Text::Encoding::UTF8);
					result = sr->ReadToEnd();
					free(buffer);
				}
			}

			if(stream) stream->Release();
			if(a) a->Release();

			return result;
		}

		static List<System::String^>^ GetDeletedEntryIDs(System::IntPtr^ outlookFolder)
		{
			List<System::String^>^ result = gcnew List<System::String^>();
			
			IUnknown* iUnkn = static_cast<IUnknown*>(outlookFolder->ToPointer());
			IMAPIFolder* fld;
			HRESULT hr;
			if(hr = iUnkn->QueryInterface(IID_IMAPIFolder, (void**)&fld))
			{
				return result;
			}
			LPMAPITABLE tbl;
			fld->GetContentsTable(0, &tbl);
			if(tbl)
			{
				// Loop until QueryRows returns 0 rows
				while(1)
				{
					LPSRowSet rows;
					tbl->QueryRows(1024, 0, &rows);
					if(rows->cRows == 0) 
					{
						MAPIFreeBuffer(rows);
						break;
					}

					for(UINT i=0;i<rows->cRows;i++)
					{
						SRow r = rows->aRow[i];
						long outlook_flag = 0;
						SBinary outlook_entryid;
						outlook_entryid.cb = 0;

						for(UINT c=0;c<r.cValues;c++)
						{
							UINT tag = HIWORD(r.lpProps[c].ulPropTag);
							_PV val = r.lpProps[c].Value;

							if(tag == 0x8019) // Message Flags
							{
								outlook_flag = val.l;
							}
							if(tag == 0x0FFF) // ENTRY_ID
							{
								outlook_entryid = val.bin;
							}
						}

						if(outlook_entryid.cb && (outlook_flag & 4))
						{
							System::Text::StringBuilder^ sb = gcnew System::Text::StringBuilder();
							for(unsigned int i=0;i<outlook_entryid.cb;i++)
							{
								sb->AppendFormat("{0:X2}", outlook_entryid.lpb[i]);
							}
							result->Add(sb->ToString());
						}

					}
					MAPIFreeBuffer(rows);
				}

				tbl->Release();
			}
			fld->Release();

			return result;
		}
	};
}
