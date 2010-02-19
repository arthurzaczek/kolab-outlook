// OutlookKolapMAPIHelper.h

#pragma once

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;

namespace OutlookKolapMAPIHelper {

	public ref class IMAPFolderHelper
	{
	public:
		List<System::String^>^ GetDeletedEntryIDs(System::IntPtr^ outlookFolder)
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
							for(int i=0;i<outlook_entryid.cb;i++)
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
