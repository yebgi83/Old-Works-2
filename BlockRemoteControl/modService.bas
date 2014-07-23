Attribute VB_Name = "modService"
Private Function GetService(ByVal serviceName As String) As Object
    Dim objWMI As Object
    Set objWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\.\root\cimv2")
    
    Dim objService As Object
    Set objService = objWMI.Get("Win32_Service.Name='" + serviceName + "'")
    
    Set GetService = objService
End Function

Private Function GetDependentServices(ByVal serviceName As String) As Object
    Dim objWMI As Object
    Set objWMI = GetObject("winmgmts:{impersonationLevel=impersonate}!\\.\root\cimv2")
    
    Dim objDependentServices  As Object
    Set objDependent = objWMI.ExecQuery("Associators of {Win32_Service.Name='" + serviceName + "'} Where AssocClass=Win32_DependentService Role=Antecedent")

    Set GetDependentServices = objDependent
End Function

Private Function GetMessageByCode(ByVal code) As String
    Select Case code
        Case 0: GetMessageByCode = "Success"
        Case 1: GetMessageByCode = "Not Supported"
        Case 2: GetMessageByCode = "Access Denided"
        Case 3: GetMessageByCode = "Dependent Services Running"
        Case 4: GetMessageByCode = "Invalid Service Control"
        Case 5: GetMessageByCode = "Service Cannot Accept Control"
        Case 6: GetMessageByCode = "Service Not Active"
        Case 7: GetMessageByCode = "Service Request timeout"
        Case 8: GetMessageByCode = "Unknown Failure"
        Case 9: GetMessageByCode = "Path Not Found"
        Case 10: GetMessageByCode = "Service Already Stopped"
        Case 11: GetMessageByCode = "Service Database Locked"
        Case 12: GetMessageByCode = "Service Dependency Deleted"
        Case 13: GetMessageByCode = "Service Dependency Failure"
        Case 14: GetMessageByCode = "Service Disabled"
        Case 15: GetMessageByCode = "Service Logon Failed"
        Case 16: GetMessageByCode = "Service Makred For Deletion"
        Case 17: GetMessageByCode = "Service No Thread"
        Case 18: GetMessageByCode = "Status Circular Dependency"
        Case 19: GetMessageByCode = "Status Duplicate Name"
        Case 20: GetMessageByCode = "Status - Invalid Name"
        Case 21: GetMessageByCode = "Status - Invalid Parameter"
        Case 22: GetMessageByCode = "Status - Invalid Service Account"
        Case 23: GetMessageByCode = "Status - Service Exists"
        Case 24: GetMessageByCode = "Service Already Paused"
        Case Else: GetMessageByCode = ""
    End Select
    
End Function

Public Function StartService(ByVal serviceName As String, ByRef errMsg As String) As Boolean
    Dim objService As Object
    Set objService = GetService(serviceName)
    
    Dim retVal As Long
    
    retVal = objService.StartService()
    errMsg = GetMessageByCode(retVal)
    
    If retVal = 0 Then
        StartService = True
    Else
        StartService = False
    End If
    
End Function

Public Function StopService(ByVal serviceName As String, ByRef errMsg As String) As Boolean
    Dim objService As Object
    Set objService = GetService(serviceName)
    
    Dim retVal As Long
    
    retVal = objService.StopService()
    
    '종속 서비스가 있는 경우
    If retVal = 3 Then
        Dim objDependentService As Object
        
        For Each objDependentService In GetDependentServices(objService.Name)
            If objDependentService.StopService = 0 Then
                Do
                    '서비스의 갱신된 상태를 가져와서 정지되었는지 확인한다.
                    Set objDependentService = GetService(objDependentService.Name)
                    If objDependentService.Started = False Then Exit Do
                Loop
            End If
        Next
        
        retVal = objService.StopService()
    End If
    
    errMsg = GetMessageByCode(retVal)
    
    If retVal = 0 Then
        StopService = True
    Else
        StopService = False
    End If
End Function

Public Function IsServiceRunning(ByVal serviceName As String) As Boolean
    Dim objService As Object
    Set objService = GetService(serviceName)
    
    IsServiceRunning = objService.Started
End Function
