using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using outlook = Microsoft.Office.Interop.Outlook;

namespace OutlookKolab.Kolab
{
    [ComVisible(false)]
    [ComImport()]
    [Guid("00020307-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMessage : IMAPIProp
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetAttachmentTable();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OpenAttach();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int CreateAttach();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DeleteAttach();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetRecipientTable();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ModifyRecipients();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SubmitMessage();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetReadFlag();
    }
    [ComVisible(false)]
    [ComImport()]
    [Guid("0002030C-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMAPIFolder : IMAPIContainer
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int CreateMessage(
            IntPtr interf,
            uint uFlags,
            [MarshalAs(UnmanagedType.Interface)] 
                    ref IMessage pMsg
        );
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int CopyMessages();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int CreateFolder();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int CopyFolder();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DeleteFolder();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetReadFlags();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMessageStatus();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetMessageStatus();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SaveContentsSort();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EmptyFolder();
    }
    [ComVisible(false)]
    [ComImport()]
    [Guid("0002030B-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMAPIContainer : IMAPIProp
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetContentsTable(uint uFlags, [MarshalAs(UnmanagedType.Interface), Out] out outlook.Table tbl);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetHierarchyTable();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OpenEntry();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetSearchCriteria();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetSearchCriteria();
    }
    [ComVisible(false)]
    [ComImport()]
    [Guid("00020303-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMAPIProp
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetLastError();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SaveChanges(
            uint uFlags
        );
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetProps();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetPropList();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OpenProperty();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetProps();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DeleteProps();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int CopyTo();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int CopyProps();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetNamesFromIDs();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetIDsFromNames();
    }
}
