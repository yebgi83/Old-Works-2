Attribute VB_Name = "modPort"
Option Explicit

Private Declare Function GetTcpTable Lib "iphlpapi.dll" (ByRef pTcpTable As Any, ByRef dwSize As Long, ByVal bOrder As Long) As Long
Private Declare Function ntohs Lib "ws2_32.dll" (ByVal netshort As Long) As Long

Private Const FIELD_STATE = 1
Private Const FIELD_LOCALADDRESS = 2
Private Const FIELD_LOCALPORT = 3
Private Const FIELD_REMOTEADDRESS = 4
Private Const FIELD_REMOTEPORT = 5

Public Const MIB_TCP_STATE_NONE = 0
Public Const MIB_TCP_STATE_CLOSED = 1
Public Const MIB_TCP_STATE_LISTEN = 2
Public Const MIB_TCP_STATE_SYN_SENT = 3
Public Const MIB_TCP_STATE_SYN_RCVD = 4
Public Const MIB_TCP_STATE_ESTAB = 5
Public Const MIB_TCP_STATE_FIN_WAIT1 = 6
Public Const MIB_TCP_STATE_FIN_WAIT2 = 7
Public Const MIB_TCP_STATE_CLOSE_WAIT = 8
Public Const MIB_TCP_STATE_CLOSING = 9
Public Const MIB_TCP_STATE_LAST_ACK = 10
Public Const MIB_TCP_STATE_TIME_WAIT = 11
Public Const MIB_TCP_STATE_DELETE_TCB = 12

Public Function IsPortUsing(ByVal port As Long) As Boolean
    On Error GoTo ErrTrap
    
    'TCP 테이블의 크기를 구한다.
    Dim dwSize As Long
    
    Call GetTcpTable(Empty, dwSize, True)
    
    If dwSize = 0 Then
        Exit Function
    End If
    
    'TCP 테이블을 가져온다.
    ReDim buffer(0 To (dwSize / 4) - 1) As Long
    
    If GetTcpTable(buffer(0), dwSize, True) <> 0 Then
        Exit Function
    End If
    
    '원격 데스크탑에서 사용하는 포트를 찾으면 True, 그렇지 않으면 False를 리턴한다.
    Dim dwEntries As Long
    dwEntries = buffer(0)
    
    Dim index As Integer
    For index = 0 To dwEntries - 1
        Dim dwState As Long
        Dim dwLocalPort As Long
        Dim dwRemotePort As Long
        Dim dwPID As Long
        
        dwState = buffer((5 * index) + FIELD_STATE)
        dwLocalPort = ntohs(buffer((5 * index) + FIELD_LOCALPORT))
        
        If port = dwLocalPort Then
            If dwState <> MIB_TCP_STATE_CLOSED And dwState <> MIB_TCP_STATE_LISTEN Then
                IsPortUsing = True
                Exit Function
            End If
        End If
    Next index
    
ErrTrap:
    IsPortUsing = False
End Function
