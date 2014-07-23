using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GetOwnerNameTest
{
    static public class ServiceNative
    {
        public const uint SC_ENUM_PROCESS_INFO = 0;
        public const uint SC_MANAGER_ENUMERATE_SERVICE = 4;
        
        public const uint SERVICE_WIN32 = 0x30;
        public const uint SERVICE_DRIVER = 0xb;
        public const uint SERVICE_ACTIVE = 1;
        
        [StructLayout(LayoutKind.Sequential)]
        public struct ENUM_SERVICE_STATUS_PROCESS
        {
            public String lpServiceName;
            public String lpDisplayName;
            public SERVICE_STATUS_PROCESS ServiceStatusProcess;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_STATUS_PROCESS
        {
            public uint dwServiceType;
            public uint dwCurrentState;
            public uint dwControlAccepted;
            public uint dwWin32ExitCode;
            public uint dwServiceSpecificExcitCode;
            public uint dwCheckPoint;
            public uint dwWaitHint;
            public uint dwProcessId;
            public uint dwServiceFlags;
        }
        
        [DllImport("advapi32.dll")]
        static extern public IntPtr OpenSCManager(String lpMachineName, String lpDatabaseName, uint dwDesiredAccess);
        
        [DllImport("advapi32.dll")]
        static extern public Boolean CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll")]
        static extern public Boolean EnumServicesStatusEx(IntPtr hSCManager, uint infoLevel, uint dwServiceType, uint dwServiceState, IntPtr lpServices, uint cbBufSize, out uint pcbBytesNeeded, out uint lpServicesReturned, IntPtr lpResumeHandle, String pszGroupName);
        
        [DllImport("advapi32.dll")]
        static extern public Boolean EnumServicesStatusEx(IntPtr hSCManager, uint infoLevel, uint dwServiceType, uint dwServiceState, IntPtr lpServices, uint cbBufSize, IntPtr pcbBytesNeeded, out uint lpServicesReturned, IntPtr lpResumeHandle, String pszGroupName);
    }
}
