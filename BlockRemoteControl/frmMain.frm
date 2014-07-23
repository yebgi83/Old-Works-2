VERSION 5.00
Begin VB.Form frmMain 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "원격 데스크탑 감지"
   ClientHeight    =   2655
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   8655
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   2655
   ScaleWidth      =   8655
   StartUpPosition =   2  'CenterScreen
   Begin VB.CommandButton m_cmdServiceOnOff 
      Caption         =   "서비스 켜기"
      Height          =   495
      Left            =   7215
      TabIndex        =   10
      Top             =   585
      Width           =   1215
   End
   Begin VB.Timer m_tmrRefresh 
      Interval        =   100
      Left            =   8040
      Top             =   1710
   End
   Begin VB.Frame m_FrameInformation 
      Caption         =   "Information"
      Height          =   1785
      Left            =   90
      TabIndex        =   1
      Top             =   420
      Width           =   8460
      Begin VB.Label m_lblStatus 
         Caption         =   "Label1"
         ForeColor       =   &H00FF0000&
         Height          =   240
         Left            =   2085
         TabIndex        =   9
         Top             =   1425
         Width           =   2865
      End
      Begin VB.Label m_lblRemotePortStatus 
         Caption         =   "Label1"
         ForeColor       =   &H00FF0000&
         Height          =   300
         Left            =   2085
         TabIndex        =   8
         Top             =   1080
         Width           =   2865
      End
      Begin VB.Label m_lblRemotePort 
         Caption         =   "Label1"
         ForeColor       =   &H00FF0000&
         Height          =   300
         Left            =   2085
         TabIndex        =   7
         Top             =   690
         Width           =   2865
      End
      Begin VB.Label m_lblServiceStatus 
         Caption         =   "Label1"
         ForeColor       =   &H00FF0000&
         Height          =   300
         Left            =   2085
         TabIndex        =   6
         Top             =   330
         Width           =   2865
      End
      Begin VB.Label Label 
         Caption         =   "현재 진행 상태 :"
         Height          =   180
         Index           =   3
         Left            =   120
         TabIndex        =   5
         Top             =   1425
         Width           =   1980
      End
      Begin VB.Label Label 
         Caption         =   "원격 서비스 포트 상태 :"
         Height          =   210
         Index           =   2
         Left            =   120
         TabIndex        =   4
         Top             =   1050
         Width           =   1980
      End
      Begin VB.Label Label 
         Caption         =   "원격 서비스 포트 번호 :"
         Height          =   210
         Index           =   1
         Left            =   120
         TabIndex        =   3
         Top             =   690
         Width           =   1980
      End
      Begin VB.Label Label 
         Caption         =   "원격 서비스 상태 :"
         Height          =   210
         Index           =   0
         Left            =   120
         TabIndex        =   2
         Top             =   330
         Width           =   1980
      End
   End
   Begin VB.Label m_lblMsg 
      Caption         =   "Message"
      Height          =   345
      Left            =   105
      TabIndex        =   11
      Top             =   2340
      Width           =   8355
   End
   Begin VB.Label m_lblHelp 
      Caption         =   "프로그램실행 -> 원격서비스 중지 -> 포트감시 -> 원격연결 확인 -> 메시지 -> 프로그램 종료"
      BeginProperty Font 
         Name            =   "굴림"
         Size            =   9
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   225
      Left            =   105
      TabIndex        =   0
      Top             =   120
      Width           =   8490
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Const REMOTE_DESKTOP_SERVICENAME = "TermService"

Private Sub Form_Activate()
    m_lblRemotePort.Caption = GetRemotePort()
End Sub

Private Sub m_cmdServiceOnOff_Click()
    Dim errMsg As String
    Dim isSuccess As Boolean
        
    If modService.IsServiceRunning(REMOTE_DESKTOP_SERVICENAME) = True Then
        If modService.StopService(REMOTE_DESKTOP_SERVICENAME, errMsg) = True Then
            m_lblMsg.Caption = Time$ + " 서비스 끄기 성공"
        Else
            m_lblMsg.Caption = Time$ + " 서비스 끄기 실패 (" + errMsg + ")"
        End If
    Else
        If modService.StartService(REMOTE_DESKTOP_SERVICENAME, errMsg) = True Then
            m_lblMsg.Caption = Time$ + " 서비스 켜기 성공"
        Else
            m_lblMsg.Caption = Time$ + " 서비스 켜기 실패 (" + errMsg + ")"
        End If
    End If

    Call DispRefresh
End Sub

Private Sub m_tmrRefresh_Timer()
    Call DispRefresh
End Sub

Private Sub DispRefresh()
    '원격 데스크탑의 서비스 상태를 표시한다.
    If modService.IsServiceRunning(REMOTE_DESKTOP_SERVICENAME) = True Then
        m_cmdServiceOnOff.Caption = "서비스 끄기"
        m_lblServiceStatus.Caption = "Running"
    Else
        m_cmdServiceOnOff.Caption = "서비스 켜기"
        m_lblServiceStatus.Caption = "Stopped"
    End If
    
    '원격 데스크탑의 포트 번호를 표시한다.
    m_lblRemotePort.Caption = GetRemotePort()
    
    '포트의 상태를 표시한다.
    If modPort.IsPortUsing(Val(m_lblRemotePort.Caption)) = True Then
        m_lblRemotePortStatus.Caption = "Established"
        m_lblStatus.Caption = "원격 연결 확인"
    Else
        m_lblRemotePortStatus.Caption = "Listening"
        m_lblStatus.Caption = "포트 감지"
    End If
End Sub

Private Function GetRemotePort() As Long
    On Error GoTo ErrTrap
    
    '레지스트리에서 값을 가져오지 못하면 포트를 3389로 고정시킨다.
    GetRemotePort = GlobalRegistry2.GetReg("HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp\PortNumber", "3389")
    Exit Function
    
ErrTrap:
    GetRemotePort = 3389
End Function
