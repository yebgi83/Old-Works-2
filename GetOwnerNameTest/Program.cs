using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

// Author : Lee kang-yong (yebgi83@gmail.com)
namespace GetOwnerNameTest
{
    // 추가 참고 : http://ntcoder.com/bab/tag/openprocesstoken/ 
    // 추가 참고 : http://www.remkoweijnen.nl/blog/2012/11/28/get-process-id-of-a-running-service/
    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine(SetPrivilege("SeDebugPrivilege"));
            //Console.WriteLine(SetPrivilege2());
            Process.EnterDebugMode();
            
            double r1, r2, r3;

            {
                DateTime startTick = DateTime.Now;
                  
                foreach (Process process in Process.GetProcesses())
                {
                    Console.WriteLine("프로세스 이름 :" + process.ProcessName);
                    Console.WriteLine("사용자 이름 :" + GetOwnerNameWithOutWMI(process.Id));
                    Console.WriteLine("서비스 목록 :" + String.Join(",", GetServicesWithWMI(process.Id)));
                    Console.WriteLine();
                }
                        
                r1 = (DateTime.Now - startTick).TotalSeconds;
            }
    
            Console.WriteLine("기존 방법 (WMI) 완료");        
            Console.ReadKey();
            
            {
                DateTime startTick = DateTime.Now;
                  
                foreach (Process process in Process.GetProcesses())
                {
                    Console.WriteLine("프로세스 이름 :" + process.ProcessName);
                    Console.WriteLine("사용자 이름 :" + GetOwnerNameWithOutWMI(process.Id));
                    Console.WriteLine("서비스 목록 :" + String.Join(",", GetServicesWithoutWMI(process.Id)));
                    Console.WriteLine();
                }
                        
                r2 = (DateTime.Now - startTick).TotalSeconds;
            }

            Console.WriteLine("1번째 방법 (WMI 사용 안함, 프로세스 가져올 때마다 서비스 목록 읽음) 완료");
            Console.ReadKey();

            Dictionary<uint, List<String>> services = GetServicesWithoutWMI();

            {
                DateTime startTick = DateTime.Now;
                  
                foreach (Process process in Process.GetProcesses())
                {
                    Console.WriteLine("프로세스 이름 :" + process.ProcessName);
                    Console.WriteLine("사용자 이름 :" + GetOwnerNameWithOutWMI(process.Id));
                    
                    if (services.ContainsKey((uint)process.Id) == true)
                    {
                        Console.WriteLine("서비스 목록 :" + String.Join(",", services[(uint)process.Id].ToArray()));
                    }
                    
                    Console.WriteLine();
                }
                        
                r3 = (DateTime.Now - startTick).TotalSeconds;
            }
            
            Console.WriteLine("2번째 방법 (WMI 사용 안함, 서비스 목록을 미리 가져옴) 완료");
            Console.WriteLine();
            Console.WriteLine("결과");
            Console.WriteLine("WMI 사용 : " + r1 + "초");
            Console.WriteLine("WMI 사용 안함 (방법 1) : " + r2 + "초");
            Console.WriteLine("WMI 사용 안함 (방법 2) : " + r3 + "초");
            Console.ReadKey();
        }
        
        static public String[] GetServicesWithWMI(Int32 pid)
        {
            List<String> serviceList = new List<String>();
            
            using (ManagementObjectSearcher wmiSearcher = new ManagementObjectSearcher("select DisplayName from Win32_Service where ProcessId = " + pid))
            {
                ManagementObjectCollection wmiCollection = wmiSearcher.Get();
            
                foreach (ManagementObject wmiObject in wmiCollection)
                {
                    serviceList.Add(wmiObject["DisplayName"] as String);
                }
            }
            
            return serviceList.ToArray();
        }        
        
        static public Dictionary<uint, List<String>> GetServicesWithoutWMI()
        {
            IntPtr scmPtr = ServiceNative.OpenSCManager(null, null, ServiceNative.SC_MANAGER_ENUMERATE_SERVICE);
            
            if (scmPtr == IntPtr.Zero) {
                throw new ApplicationException("Unable to connect to service manager. The last error that was reported was " + new System.ComponentModel.Win32Exception().Message);
            }
            
            uint bytesNeeded = 0;
            uint servicesFound = 0;
            
            ServiceNative.EnumServicesStatusEx(scmPtr, ServiceNative.SC_ENUM_PROCESS_INFO, ServiceNative.SERVICE_WIN32, 3, IntPtr.Zero, 0, out bytesNeeded, out servicesFound, IntPtr.Zero, null);
            
            IntPtr servicesPtr = Marshal.AllocHGlobal((int)bytesNeeded);
            
            ServiceNative.EnumServicesStatusEx(scmPtr, ServiceNative.SC_ENUM_PROCESS_INFO, ServiceNative.SERVICE_WIN32, 3, servicesPtr, bytesNeeded, out bytesNeeded, out servicesFound, IntPtr.Zero, null);
            
            int serviceStructureSize = Marshal.SizeOf(typeof(ServiceNative.ENUM_SERVICE_STATUS_PROCESS));
            
            Dictionary<uint, List<String>> serviceList = new Dictionary<uint, List<String>>();
            
            for (int i = 0; i < servicesFound; i++)
            {
                ServiceNative.ENUM_SERVICE_STATUS_PROCESS service = (ServiceNative.ENUM_SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(servicesPtr, typeof(ServiceNative.ENUM_SERVICE_STATUS_PROCESS));
                
                try
                {
                    if (serviceList.ContainsKey(service.ServiceStatusProcess.dwProcessId) == false)
                    {
                        serviceList.Add(service.ServiceStatusProcess.dwProcessId, new List<string>());
                    }
                    
                    serviceList[service.ServiceStatusProcess.dwProcessId].Add(service.lpDisplayName);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    servicesPtr = (IntPtr)(servicesPtr.ToInt32() + serviceStructureSize);
                }
            }
            
            return serviceList;
        }
        
        static public String[] GetServicesWithoutWMI(Int32 pid)
        {
            IntPtr scmPtr = ServiceNative.OpenSCManager(null, null, ServiceNative.SC_MANAGER_ENUMERATE_SERVICE);
            
            if (scmPtr == IntPtr.Zero) {
                throw new ApplicationException("Unable to connect to service manager. The last error that was reported was " + new System.ComponentModel.Win32Exception().Message);
            }
            
            uint bytesNeeded = 0;
            uint servicesFound = 0;
            
            ServiceNative.EnumServicesStatusEx(scmPtr, ServiceNative.SC_ENUM_PROCESS_INFO, ServiceNative.SERVICE_WIN32, 3, IntPtr.Zero, 0, out bytesNeeded, out servicesFound, IntPtr.Zero, null);
            
            IntPtr servicesPtr = Marshal.AllocHGlobal((int)bytesNeeded);
            
            ServiceNative.EnumServicesStatusEx(scmPtr, ServiceNative.SC_ENUM_PROCESS_INFO, ServiceNative.SERVICE_WIN32, 3, servicesPtr, bytesNeeded, out bytesNeeded, out servicesFound, IntPtr.Zero, null);
            
            int serviceStructureSize = Marshal.SizeOf(typeof(ServiceNative.ENUM_SERVICE_STATUS_PROCESS));
            
            List<String> serviceList = new List<String>();
            
            for (int i = 0; i < servicesFound; i++)
            {
                ServiceNative.ENUM_SERVICE_STATUS_PROCESS service = (ServiceNative.ENUM_SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(servicesPtr, typeof(ServiceNative.ENUM_SERVICE_STATUS_PROCESS));
                
                try
                {
                    if (service.ServiceStatusProcess.dwProcessId == pid)
                    {
                        serviceList.Add(service.lpDisplayName);
                    }
                }
                catch
                {
                    continue;
                }
                finally
                {
                    servicesPtr = (IntPtr)(servicesPtr.ToInt32() + serviceStructureSize);
                }
            }
            
            return serviceList.ToArray();
        }
        
        static public String GetOwnerNameWithOutWMI_ByToken(Int32 pid)
        {
            try
            {
                IntPtr processHandle = ProcessNative.OpenProcess(0x00020000, false, pid);
                
                StringBuilder ownerName = new StringBuilder(512);
                StringBuilder refDomainName = new StringBuilder(512);

                UInt32 cchOwnerName = 512;
                UInt32 cchRefDomainName = 512;

                SecurityNative.SID_NAME_USE sidUse = default(SecurityNative.SID_NAME_USE);
                
                IntPtr tokenHandle;
                IntPtr tokenInformation;
                UInt32 tokenInformationLength = 0;
                
                // Get token.
                ProcessTokenNative.OpenProcessToken(processHandle, ProcessTokenNative.TOKEN_READ, out tokenHandle);
                
                // token user for acquire owner name of process.
                ProcessTokenNative.GetTokenInformation(tokenHandle, ProcessTokenNative.TOKEN_INFORMATION_CLASS.TokenUser, IntPtr.Zero, tokenInformationLength, out tokenInformationLength);
                
                if (tokenInformationLength > 0) 
                {
                    tokenInformation = Marshal.AllocHGlobal((int)tokenInformationLength);
                    ProcessTokenNative.GetTokenInformation(tokenHandle, ProcessTokenNative.TOKEN_INFORMATION_CLASS.TokenUser, tokenInformation, tokenInformationLength, out tokenInformationLength);
                    
                    ProcessTokenNative.TOKEN_USER tokenUser = (ProcessTokenNative.TOKEN_USER)Marshal.PtrToStructure(tokenInformation, typeof(ProcessTokenNative.TOKEN_USER));
                    SecurityNative.LookupAccountSid(null, tokenUser.User.Sid, ownerName, ref cchOwnerName, refDomainName, ref cchRefDomainName, out sidUse);
                }
                
                // Return owner name.
                return ownerName.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine (e.Message);
                return String.Empty;
            }
        }
        
        static public String GetOwnerNameWithOutWMI_ByToken2(Int32 pid)
        {
            try
            {
                IntPtr processHandle = ProcessNative.OpenProcess(0x001f0fff, true, pid);
                IntPtr tokenHandle;
                
                // Get token.
                ProcessTokenNative.OpenProcessToken(processHandle, ProcessTokenNative.TOKEN_READ, out tokenHandle);
                
                System.Security.Principal.WindowsIdentity identity = new System.Security.Principal.WindowsIdentity(tokenHandle);
                
                // Return owner name.
                return identity.Name;
            }
            catch (Exception e)
            {
                Console.WriteLine (e.Message);
                return String.Empty;
            }
        }

        /// <summary>
        /// Get owner name with WMI, It is so slow because of using WMI.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        static public String GetOwnerNameWithOutWMI(Int32 pid)
        {
            try
            {
                IntPtr processHandle = Process.GetProcessById(pid).Handle;//ProcessNative.OpenProcess(0x00020000, false, pid);
                IntPtr sidOwner, sidGroup, dacl, sacl, securityDescriptor;
                
                StringBuilder ownerName = new StringBuilder(512);
                StringBuilder refDomainName = new StringBuilder(512);
                
                UInt32 cchOwnerName = 512;
                UInt32 cchRefDomainName = 512;
                
                SecurityNative.SID_NAME_USE sidUse = default(SecurityNative.SID_NAME_USE);
                
                // Get security information for acquire owner name of process.
                SecurityNative.GetSecurityInfo(processHandle, SecurityNative.SE_OBJECT_TYPE.SE_KERNEL_OBJECT, SecurityNative.SECURITY_INFORMATION.GROUP_SECURITY_INFORMATION | SecurityNative.SECURITY_INFORMATION.OWNER_SECURITY_INFORMATION, out sidOwner, out sidGroup, out dacl, out sacl, out securityDescriptor);

                // Get owner name.
                SecurityNative.LookupAccountSid(null, sidGroup, ownerName, ref cchOwnerName, refDomainName, ref cchRefDomainName, out sidUse);
                
                // String SID
                String strSid;
                SecurityNative.ConvertSidToStringSid(sidOwner, out strSid);
                
                // Return owner name.
                return ownerName.ToString() + ", " + refDomainName.ToString() + ", " + strSid;
            }
            catch (Exception e)
            {
                Console.WriteLine (e.Message);
                return String.Empty;
            }
        }
        
        static public String GetOwnerNameWithWMI(Int32 pid)
        {
            try
            {
                using (ManagementObjectSearcher wmiSearcher = new ManagementObjectSearcher("select * from Win32_Process where ProcessId = " + pid))
                {
                    ManagementObjectCollection wmiCollection = wmiSearcher.Get();

                    foreach (ManagementObject wmiObject in wmiCollection)
                    {
                        String[] argList = new String[] { String.Empty, String.Empty };

                        if (Convert.ToInt32(wmiObject.InvokeMethod("GetOwner", argList)) == 0)
                        {
                            return argList[1] + "\\" + argList[0];
                        }
                    }

                    return "NO OWNER";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine (e.Message);
                return String.Empty;
            }
        }
        
        static public bool SetPrivilege(String lpszPrivilege)
        {
            try
            {
                const int SE_PRIVILEGE_ENABLED = 0x00000002;
            
                IntPtr tokenHandle;
                IntPtr processHandle = ProcessNative.GetCurrentProcess();
                
                ProcessTokenNative.LUID luid;
                
                ProcessTokenNative.OpenProcessToken(processHandle, ProcessTokenNative.TOKEN_ADJUST_PRIVILEGES | ProcessTokenNative.TOKEN_QUERY, out tokenHandle);
                ProcessTokenNative.LookupPrivilegeValue(null, lpszPrivilege, out luid);
                ProcessTokenNative.TOKEN_PRIVILEGES tokenPrivileges = new ProcessTokenNative.TOKEN_PRIVILEGES();
                
                IntPtr luidPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ProcessTokenNative.LUID)));
                Marshal.StructureToPtr(luid, luidPtr, true);
                
                tokenPrivileges.PrivilegeCount = 1;
                tokenPrivileges.Privilieges = new ProcessTokenNative.LUID_AND_ATTRIBUTES[1];
                tokenPrivileges.Privilieges[0].Luid = luidPtr;
                tokenPrivileges.Privilieges[0].Attributes = SE_PRIVILEGE_ENABLED;
                
                return ProcessTokenNative.AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero);
            }
            catch 
            {
                return false;
            }
        }
        
        static public bool SetPrivilege2()
        {
            Int32 previousValue;
            
            if (ProcessTokenNative.RtlAdjustPrivilege(20, 1, 0, out previousValue) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
