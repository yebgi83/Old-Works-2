Attribute VB_Name = "modService"
Public Function StartService(ByVal serviceName As String) As Boolean
    On Error GoTo ErrTrap
    
    Dim shellApp As Object
    shellApp = CreateObject("Shell.Application")
    
    If shellApp.ServiceStart(serviceName, True) = True Then
        StartService = True
        Exit Function
    End If
    
ErrTrap:
    StartService = False
End Function

Public Function StopService(ByVal serviceName As String) As Boolean
    On Error GoTo ErrTrap
    
    Dim shellApp As Object
    shellApp = CreateObject("Shell.Application")
    
    If shellApp.ServiceStop(serviceName, False) = True Then
        StopService = True
        Exit Function
    Else

ErrTrap:
    StopService = False
End Function

Public Function IsServiceRunning(ByVal serviceName As String) As Boolean
    Dim shellApp As Object
    shellApp = CreateObject("Shell.Application")
    
    IsServiceRunning = shellApp.IsServiceRunning(serviceName)
End Function
