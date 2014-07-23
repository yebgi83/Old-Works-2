VERSION 5.00
Begin VB.Form frmMain 
   Caption         =   "Form1"
   ClientHeight    =   3090
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   4680
   LinkTopic       =   "Form1"
   ScaleHeight     =   3090
   ScaleWidth      =   4680
   StartUpPosition =   3  'Windows Default
   Begin VB.Timer tmrDisturbance 
      Interval        =   100
      Left            =   75
      Top             =   570
   End
   Begin VB.Timer tmrNow 
      Interval        =   1
      Left            =   60
      Top             =   60
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub tmrDisturbance_Timer()
    Dim iCount As Integer
    
    For iCount = 1 To 1000
        Me.Caption = iCount
    Next iCount
End Sub

Private Sub tmrNow_Timer()
    If tmrNow.Tag = "" Then
        tmrNow.Tag = Str(Timer + 1)
    Else
        Dim fCurrent As Single
        fCurrent = Timer
        
        If fCurrent > CSng(tmrNow.Tag) Then
            Dim fOver As Single
            fOver = (Timer - CSng(tmrNow.Tag))
            
            tmrNow.Tag = Timer + (1 - fOver)
            Debug.Print Timer, "Interval = "; (1 - fOver)
        End If
    End If
End Sub

