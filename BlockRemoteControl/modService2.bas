Attribute VB_Name = "modService2"
'Native APIs (SERVICE)
Private Declare Function OpenSCManager Lib "advapi32.dll" Alias "OpenSCManagerA" (ByVal lpMachineName As Any, ByVal lpDatabaseName As Any, ByVal dwDesiredAccess As Long) As Long
Private Declare Function OpenService Lib "advapi32.dll" Alias "OpenServiceA" (ByVal hSCManager As Long, ByVal lpServiceName As String, ByVal dwDesiredAccess As Long) As Long
Private Declare Function ControlService Lib "advapi32.dll" (ByVal hService As Long, ByVal dwControl As Long, lpServiceStatus As SERVICE_STATUS) As Long
Private Declare Function StartServiceImpl Lib "advapi32.dll" Alias "StartServiceA" (ByVal hService As Long, ByVal dwNumServiceArgs As Long, ByVal lpServiceArgVectors As Long) As Long
Private Declare Function QueryServiceStatus Lib "advapi32.dll" (ByVal hService As Long, lpServiceStatus As SERVICE_STATUS) As Long
Private Declare Function CloseServiceHandle Lib "advapi32.dll" (ByVal hSCObject As Long) As Long
Private Declare Function EnumServicesStatus Lib "advapi32.dll" Alias "EnumServicesStatusA" (ByVal hSCManager As Long, ByVal dwServiceType As Long, ByVal dwServiceState As Long, ByRef lpServices As ENUM_SERVICE_STATUS, ByVal cbBufSize As Long, pcbBytesNeeded As Long, lpServicesReturned As Long, lpResumeHandle As Long) As Long
Private Declare Function EnumDependentServices Lib "advapi32.dll" Alias "EnumDependentServicesA" (ByVal hService As Long, ByVal dwServiceState As Long, ByRef lpServices As ENUM_SERVICE_STATUS, ByVal cbBufSize As Long, pcbBytesNeeded As Long, lpServicesReturned As Long) As Long

'Native Constants (SERVICE)
Private Const SC_MANAGER_ALL_ACCESS = &HF003F
Private Const SC_MANAGER_CONNECT = 1
Private Const SC_MANAGER_CREATE_SERVICE = 2
Private Const SC_MANAGER_ENUMERATE_SERVICE = 4
Private Const SC_MANAGER_LOCK = 8
Private Const SC_MANAGER_QUERY_LOCK_STATUS = 16
Private Const SC_MANAGER_MODIFY_BOOT_CONFIG = 32

Private Const SERVICE_NO_CHANGE = &HFFFFFFFF
Private Const SERVICE_STOPPED = 1
Private Const SERVICE_START_PENDING = 2
Private Const SERVICE_STOP_PENDING = 3
Private Const SERVICE_RUNNING = 4
Private Const SERVICE_CONTINUE_PENDING = 5
Private Const SERVICE_PAUSE_PENDING = 6
Private Const SERVICE_PAUSED = 7

Private Const SERVICE_ACCEPT_STOP = 1
Private Const SERVICE_ACCEPT_PAUSE_CONTINUE = 2
Private Const SERVICE_ACCEPT_SHUTDOWN = 4
Private Const SERVICE_ACCEPT_PARAMCHANGE = 8
Private Const SERVICE_ACCEPT_NETBINDCHANGE = 16
Private Const SERVICE_ACCEPT_HARDWAREPROFILECHANGE = 32
Private Const SERVICE_ACCEPT_POWEREVENT = 64
Private Const SERVICE_ACCEPT_SESSIONCHANGE = 128

Private Const SERVICE_CONTROL_STOP = 1
Private Const SERVICE_CONTROL_PAUSE = 2
Private Const SERVICE_CONTROL_CONTINUE = 3
Private Const SERVICE_CONTROL_INTERROGATE = 4
Private Const SERVICE_CONTROL_SHUTDOWN = 5
Private Const SERVICE_CONTROL_PARAMCHANGE = 6
Private Const SERVICE_CONTROL_NETBINDADD = 7
Private Const SERVICE_CONTROL_NETBINDREMOVE = 8
Private Const SERVICE_CONTROL_NETBINDENABLE = 9
Private Const SERVICE_CONTROL_NETBINDDISABLE = 10
Private Const SERVICE_CONTROL_DEVICEEVENT = 11
Private Const SERVICE_CONTROL_HARDWAREPROFILECHANGE = 12
Private Const SERVICE_CONTROL_POWEREVENT = 13
Private Const SERVICE_CONTROL_SESSIONCHANGE = 14

Private Const SERVICE_ACTIVE = 1
Private Const SERVICE_INACTIVE = 2
Private Const SERVICE_STATE_ALL = 3

Private Const SERVICE_QUERY_CONFIG = 1
Private Const SERVICE_CHANGE_CONFIG = 2
Private Const SERVICE_QUERY_STATUS = 4
Private Const SERVICE_ENUMERATE_DEPENDENTS = 8
Private Const SERVICE_START = 16
Private Const SERVICE_STOP = 32
Private Const SERVICE_PAUSE_CONTINUE = 64
Private Const SERVICE_INTERROGATE = 128
Private Const SERVICE_USER_DEFINED_CONTROL = 256
Private Const SERVICE_RUNS_IN_SYSTEM_PROCESS = 1

Private Const SERVICE_CONFIG_DESCRIPTION = 1
Private Const SERVICE_CONFIG_FAILURE_ACTIONS = 2
Private Const SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 3
Private Const SERVICE_CONFIG_FAILURE_ACTIONS_FLAG = 4
Private Const SERVICE_CONFIG_SERVICE_SID_INFO = 5
Private Const SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO = 6
Private Const SERVICE_CONFIG_PRESHUTDOWN_INFO = 7
Private Const SERVICE_CONFIG_TRIGGER_INFO = 8
Private Const SERVICE_CONFIG_PREFERRED_NODE = 9
Private Const SERVICE_CONFIG_RUNLEVEL_INFO = 10
        
'Native Type (SERVICE)
Private Type SERVICE_STATUS
    dwServiceType               As Long
    dwCurrentState              As Long
    dwControlAccepted           As Long
    dwWin32ExitCode             As Long
    dwServiceSpecificExitCode   As Long
    dwCheckPoint                As Long
    dwWaitHint                  As Long
End Type

Private Type ENUM_SERVICE_STATUS
    lpServiceName           As String
    lpDisplayName           As String
    ServiceStatusProcess    As SERVICE_STATUS
End Type

Private Function GetService(ByVal serviceName As String, ByVal dwDesiredAccess As Long) As Long
    On Error GoTo ErrTrap
    
    Dim hSCManager As Long
    hSCManager = OpenSCManager(0&, 0&, SC_MANAGER_ALL_ACCESS)

    If hSCManager = 0 Then
        Exit Function
    End If
    
    GetService = OpenService(hSCManager, serviceName, dwDesiredAccess)
    
ErrTrap:
    Call CloseServiceHandle(hSCManager)
End Function

Public Sub StartService(ByVal serviceName As String)
    Dim hService As Long
    hService = GetService(serviceName, SERVICE_START)
    
    If hService = 0 Then
        Exit Sub
    End If
    
    Call StartServiceImpl(hService, 0, 0)
    
ErrTrap:
    Call CloseServiceHandle(hSCManager)
End Sub

Public Sub StopService(ByVal serviceName As String)
    Dim hService As Long
    hService = GetService(serviceName, SERVICE_STOP Or SERVICE_QUERY_STATUS)
    
    If hService = 0 Then
        Exit Sub
    End If
    
    Dim serviceStatus As SERVICE_STATUS
    Call ControlService(hService, SERVICE_CONTROL_STOP, serviceStatus)
    
ErrTrap:
    Call CloseServiceHandle(hSCManager)
End Sub

Public Function IsServiceRunning(ByVal serviceName As String)
    If GetServiceStatus(serviceName) = "RUNNING" Then
        IsServiceRunning = True
    Else
        IsServiceRunning = False
    End If
End Function

Public Function GetDependentServices(ByVal serviceName As String)
    Const STRUCT_SIZE = 36

    Dim hService As Long
    hService = GetService(serviceName, SERVICE_ENUMERATE_DEPENDENTS)
    
    If hService = 0 Then
        Exit Function
    End If
    
    Dim dwBytesNeeded As Long
    Dim dwCount As Long
    
    ReDim enumServices(0) As ENUM_SERVICE_STATUS
    Debug.Print EnumDependentServices(hService, SERVICE_STATE_ALL, enumServices(0), 0&, dwBytesNeeded, dwCount)
    
    If dwBytesNeeded = 0 Then Exit Function
    
    Dim entries As Long
    entries = dwBytesNeeded / (STRUCT_SIZE + 1)
    
    ReDim enumServices(0 To entries)
    Debug.Print EnumDependentServices(hService, SERVICE_STATE_ALL, enumServices(0), entries * STRUCT_SIZE, dwBytesNeeded, dwCount)
End Function

Public Function GetServiceStatus(ByVal serviceName As String) As String
    Dim hService As Long
    hService = GetService(serviceName, SERVICE_QUERY_STATUS)
    
    If hService = 0 Then
        Exit Function
    End If
    
    Dim serviceStatus As SERVICE_STATUS
    Call QueryServiceStatus(hService, serviceStatus)
    
    Select Case serviceStatus.dwCurrentState
        Case SERVICE_STOPPED
            GetServiceStatus = "STOPPED"
            
        Case SERVICE_START_PENDING
            GetServiceStatus = "START_PENDING"
        
        Case SERVICE_STOP_PENDING
            GetServiceStatus = "STOP_PENDING"
        
        Case SERVICE_RUNNING
            GetServiceStatus = "RUNNING"
        
        Case SERVICE_CONTINUE_PENDING
            GetServiceStatus = "CONTINUE_PENDING"
            
        Case SERVICE_PAUSE_PENDING
            GetServiceStatus = "PAUSE_PENDING"
            
        Case SERVICE_PAUSED
            GetServiceStatus = "PAUSED"
            
        Case Else
            GetServiceStatus = ""
    End Select
    
ErrTrap:
    Call CloseServiceHandle(hService)
End Function
