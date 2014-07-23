using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GetOwnerNameTest
{
    static class ProcessTokenNative
    {
        public const UInt32 STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const UInt32 STANDARD_RIGHTS_READ = 0x00020000;
        public const UInt32 TOKEN_ASSIGN_PRIMARY = 0x0001;
        public const UInt32 TOKEN_DUPLICATE = 0x0002;
        public const UInt32 TOKEN_IMPERSONATE = 0x0004;
        public const UInt32 TOKEN_QUERY = 0x0008;
        public const UInt32 TOKEN_QUERY_SOURCE = 0x0010;
        public const UInt32 TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const UInt32 TOKEN_ADJUST_GROUPS = 0x0040;
        public const UInt32 TOKEN_ADJUST_DEFAULT = 0x0080;
        public const UInt32 TOKEN_ADJUST_SESSIONID = 0x0100;
        public const UInt32 TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);

        public enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin
        };
        
        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public UInt32 PrivilegeCount;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=1)]
            public LUID_AND_ATTRIBUTES[] Privilieges;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_USER
        {
            public SID_AND_ATTRIBUTES User;
        }

        [StructLayout(LayoutKind.Sequential)] 
        public struct LUID_AND_ATTRIBUTES
        {
            public IntPtr Luid;
            public int Attributes;
        } 

        [StructLayout(LayoutKind.Sequential)]
        public struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public int Attributes;
        } 

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID 
        {
            public uint LowPart;
            public int HighPart;
        }

        [DllImport("advapi32.dll")]
        public static extern bool OpenProcessToken(IntPtr processHandle, UInt32 desiredAccess, out IntPtr tokenHandle);
        
        [DllImport("advapi32.dll")]
        public static extern bool GetTokenInformation(IntPtr tokenHandle, TOKEN_INFORMATION_CLASS tokenInformationClass, IntPtr tokenInformation, uint tokenInformationLength, out uint returnLength);
        
        [DllImport("advapi32.dll")]
        public static extern bool LookupPrivilegeValue(String lpSystemName, String lpName, out LUID lpLUID);
        
        [DllImport("advapi32.dll")]
        public static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivileges, ref TOKEN_PRIVILEGES newState, UInt32 buffferLength, IntPtr previousState, IntPtr returnLengthInBytes);

        [DllImport("advapi32.dll")]
        public static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivileges, ref TOKEN_PRIVILEGES newState, UInt32 buffferLength, ref TOKEN_PRIVILEGES previousState, out Int32 returnLengthInBytes);
        
        [DllImport("ntdll.dll")]
        public static extern int RtlAdjustPrivilege(Int32 privilege, Int32 bEnablePrivilege, Int32 isThreadPrivilege, out Int32 previousValue);
    }
}
