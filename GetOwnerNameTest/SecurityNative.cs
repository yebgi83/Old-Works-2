using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GetOwnerNameTest
{
    static public class SecurityNative
    {
        public enum SE_OBJECT_TYPE
        {
            SE_UNKNOWN_OBJECT_TYPE = 0,
            SE_FILE_OBJECT,
            SE_SERVICE,
            SE_PRINTER,
            SE_REGISTRY_KEY,
            SE_LMSHARE,
            SE_KERNEL_OBJECT,
            SE_WINDOW_OBJECT,
            SE_DS_OBJECT,
            SE_DS_OBJECT_ALL,
            SE_PROVIDER_DEFINED_OBJECT,
            SE_WMIGUID_OBJECT,
            SE_REGISTRY_WOW64_32KEY
        };
        
        [Flags]
        public enum SECURITY_INFORMATION 
        {
            OWNER_SECURITY_INFORMATION = 1,
            GROUP_SECURITY_INFORMATION = 2,
            DACL_SECURITY_INFORMATION = 4,
            SACL_SECURITY_INFORMATION = 8
        };
        
        [DllImport("advapi32.dll")]
        static extern public Int32 GetSecurityInfo(IntPtr handle, SE_OBJECT_TYPE objectType, SECURITY_INFORMATION securityInfo, out IntPtr sidOwner, out IntPtr sidGroup, out IntPtr dacl, out IntPtr sacl, out IntPtr securityDescriptor);
   
        public enum SID_NAME_USE
        {
            SidTypeUser = 1,
            SidTypeGroup,
            SidTypeDomain,
            SidTypeAlias,
            SidTypeWellKnownGroup,
            SidTypeDeletedAccount,
            SidTypeInvalid,
            SidTypeUnknown,
            SidTypeComputer
        };
        
        [DllImport("advapi32.dll")]
        static extern public Int32 LookupAccountSid(string lpSystemName, IntPtr sid, StringBuilder lpName, ref uint cchName, StringBuilder refDomainName, ref uint cchRefDomainName, out SID_NAME_USE peUse);
        
        [DllImport("advapi32.dll")]
        static extern public Boolean ConvertSidToStringSid(IntPtr sid, out String strSid);
    };
}
