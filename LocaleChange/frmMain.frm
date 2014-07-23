VERSION 5.00
Begin VB.Form frmMain 
   AutoRedraw      =   -1  'True
   BorderStyle     =   1  'Fixed Single
   Caption         =   "테스트 - 국가 및 언어 옵션 변환"
   ClientHeight    =   1155
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   6405
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1155
   ScaleWidth      =   6405
   StartUpPosition =   3  'Windows Default
   Tag             =   "테스트 - 국가 및 언어 옵션 변환"
   Begin VB.CommandButton cmdSetLocale 
      Caption         =   "대한민국(0412)"
      Height          =   495
      Index           =   4
      Left            =   5085
      TabIndex        =   6
      Tag             =   "1042"
      Top             =   585
      Width           =   1215
   End
   Begin VB.CommandButton cmdSetLocale 
      Caption         =   "일본(0411)"
      Height          =   495
      Index           =   3
      Left            =   3840
      TabIndex        =   5
      Tag             =   "1041"
      Top             =   585
      Width           =   1215
   End
   Begin VB.CommandButton cmdSetLocale 
      Caption         =   "대만(0404)"
      Height          =   495
      Index           =   2
      Left            =   2595
      TabIndex        =   4
      Tag             =   "1028"
      Top             =   585
      Width           =   1215
   End
   Begin VB.CommandButton cmdSetLocale 
      Caption         =   "프랑스(040c)"
      Height          =   495
      Index           =   1
      Left            =   1350
      TabIndex        =   3
      Tag             =   "1036"
      Top             =   585
      Width           =   1215
   End
   Begin VB.CommandButton cmdSetLocale 
      Caption         =   "미국(0409)"
      Height          =   495
      Index           =   0
      Left            =   105
      TabIndex        =   0
      Tag             =   "1033"
      Top             =   585
      Width           =   1215
   End
   Begin VB.Label lblSystemLocale 
      AutoSize        =   -1  'True
      Caption         =   "현재 로케일"
      ForeColor       =   &H00FF0000&
      Height          =   195
      Left            =   2610
      TabIndex        =   2
      Top             =   180
      Width           =   945
   End
   Begin VB.Label Label 
      AutoSize        =   -1  'True
      Caption         =   "국가 및 언어 옵션 (현재 설정) :"
      Height          =   195
      Left            =   150
      TabIndex        =   1
      Top             =   180
      Width           =   2385
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Declare Function GetVersion Lib "kernel32" () As Long
Private Declare Function GetLocaleInfo Lib "kernel32" Alias "GetLocaleInfoW" (ByVal Locale As Long, ByVal LCType As Long, ByVal lpLCData As Long, ByVal cchData As Long) As Long

Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As Long, ByVal wMsg As Long, ByVal wParam As Long, lParam As Any) As Long

'-----------------------------------------------------------------------------------------------
'               Constants for setting and getting the information of system locale.
'-----------------------------------------------------------------------------------------------
Private Const LOCALE_NOUSEROVERRIDE         As Long = &H80000000
Private Const LOCALE_USE_CP_ACP             As Long = &H40000000

'#if(WINVER >= 400)
Private Const LOCALE_RETURN_NUMBER          As Long = &H20000000
'#endif /* WINVER >= 400

Private Const LOCALE_ILANGUAGE              As Long = &H1     'language id
Private Const LOCALE_SLANGUAGE              As Long = &H2     'localized name of language
Private Const LOCALE_SENGLANGUAGE           As Long = &H1001  'English name of language
Private Const LOCALE_SABBREVCTRYNAME        As Long = &H7     'abbreviated country name
Private Const LOCALE_SNATIVECTRYNAME        As Long = &H8     'native name of country

Private Const LOCALE_ICOUNTRY               As Long = &H5     'country code
Private Const LOCALE_SCOUNTRY               As Long = &H6     'localized name of country
Private Const LOCALE_SENGCOUNTRY            As Long = &H1002  'English name of country
Private Const LOCALE_SABBREVLANGNAME        As Long = &H3     'abbreviated language name
Private Const LOCALE_SNATIVELANGNAME        As Long = &H4     'native name of language
Private Const LOCALE_IGEOID                 As Long = &H5B    'geographical location id

Private Const LOCALE_IDEFAULTLANGUAGE       As Long = &H9     'default language id
Private Const LOCALE_IDEFAULTCOUNTRY        As Long = &HA     'default country code
Private Const LOCALE_IDEFAULTCODEPAGE       As Long = &HB     'default oem code page
Private Const LOCALE_IDEFAULTANSICODEPAGE   As Long = &H1004  'default ansi code page
Private Const LOCALE_IDEFAULTMACCODEPAGE    As Long = &H1011  'default mac code page

Private Const LOCALE_SLIST                  As Long = &HC     'list item separator
Private Const LOCALE_IMEASURE               As Long = &HD     '0 = metric, 1 = US

Private Const LOCALE_SDECIMAL               As Long = &HE     'decimal separator
Private Const LOCALE_STHOUSAND              As Long = &HF     'thousand separator
Private Const LOCALE_SGROUPING              As Long = &H10    'digit grouping
Private Const LOCALE_IDIGITS                As Long = &H11    'number of fractional digits
Private Const LOCALE_ILZERO                 As Long = &H12    'leading zeros for decimal
Private Const LOCALE_INEGNUMBER             As Long = &H1010  'negative number mode
Private Const LOCALE_SNATIVEDIGITS          As Long = &H13    'native ascii 0-9

Private Const LOCALE_SCURRENCY              As Long = &H14    'local monetary symbol
Private Const LOCALE_SINTLSYMBOL            As Long = &H15    'intl monetary symbol
Private Const LOCALE_SMONDECIMALSEP         As Long = &H16    'monetary decimal separator
Private Const LOCALE_SMONTHOUSANDSEP        As Long = &H17    'monetary thousand separator
Private Const LOCALE_SMONGROUPING           As Long = &H18    'monetary grouping
Private Const LOCALE_ICURRDIGITS            As Long = &H19    '# local monetary digits
Private Const LOCALE_IINTLCURRDIGITS        As Long = &H1A    '# intl monetary digits
Private Const LOCALE_ICURRENCY              As Long = &H1B    'positive currency mode
Private Const LOCALE_INEGCURR               As Long = &H1C    'negative currency mode

Private Const LOCALE_SDATE                  As Long = &H1D    'date separator
Private Const LOCALE_STIME                  As Long = &H1E    'time separator
Private Const LOCALE_SSHORTDATE             As Long = &H1F    'short date format string
Private Const LOCALE_SLONGDATE              As Long = &H20    'long date format string
Private Const LOCALE_STIMEFORMAT            As Long = &H1003  'time format string
Private Const LOCALE_IDATE                  As Long = &H21    'short date format ordering
Private Const LOCALE_ILDATE                 As Long = &H22    'long date format ordering
Private Const LOCALE_ITIME                  As Long = &H23    'time format specifier
Private Const LOCALE_ITIMEMARKPOSN          As Long = &H1005  'time marker position
Private Const LOCALE_ICENTURY               As Long = &H24    'century format specifier (short date)
Private Const LOCALE_ITLZERO                As Long = &H25    'leading zeros in time field
Private Const LOCALE_IDAYLZERO              As Long = &H26    'leading zeros in day field (short date)
Private Const LOCALE_IMONLZERO              As Long = &H27    'leading zeros in month field (short date)
Private Const LOCALE_S1159                  As Long = &H28    'AM designator
Private Const LOCALE_S2359                  As Long = &H29    'PM designator

Private Const LOCALE_ICALENDARTYPE          As Long = &H1009  'type of calendar specifier
Private Const LOCALE_IOPTIONALCALENDAR      As Long = &H100B  'additional calendar types specifier
Private Const LOCALE_IFIRSTDAYOFWEEK        As Long = &H100C  'first day of week specifier
Private Const LOCALE_IFIRSTWEEKOFYEAR       As Long = &H100D  'first week of year specifier

Private Const LOCALE_SDAYNAME1              As Long = &H2A    'long name for Monday
Private Const LOCALE_SDAYNAME2              As Long = &H2B    'long name for Tuesday
Private Const LOCALE_SDAYNAME3              As Long = &H2C    'long name for Wednesday
Private Const LOCALE_SDAYNAME4              As Long = &H2D    'long name for Thursday
Private Const LOCALE_SDAYNAME5              As Long = &H2E    'long name for Friday
Private Const LOCALE_SDAYNAME6              As Long = &H2F    'long name for Saturday
Private Const LOCALE_SDAYNAME7              As Long = &H30    'long name for Sunday
Private Const LOCALE_SABBREVDAYNAME1        As Long = &H31    'abbreviated name for Monday
Private Const LOCALE_SABBREVDAYNAME2        As Long = &H32    'abbreviated name for Tuesday
Private Const LOCALE_SABBREVDAYNAME3        As Long = &H33    'abbreviated name for Wednesday
Private Const LOCALE_SABBREVDAYNAME4        As Long = &H34    'abbreviated name for Thursday
Private Const LOCALE_SABBREVDAYNAME5        As Long = &H35    'abbreviated name for Friday
Private Const LOCALE_SABBREVDAYNAME6        As Long = &H36    'abbreviated name for Saturday
Private Const LOCALE_SABBREVDAYNAME7        As Long = &H37    'abbreviated name for Sunday
Private Const LOCALE_SMONTHNAME1            As Long = &H38    'long name for January
Private Const LOCALE_SMONTHNAME2            As Long = &H39    'long name for February
Private Const LOCALE_SMONTHNAME3            As Long = &H3A    'long name for March
Private Const LOCALE_SMONTHNAME4            As Long = &H3B    'long name for April
Private Const LOCALE_SMONTHNAME5            As Long = &H3C    'long name for May
Private Const LOCALE_SMONTHNAME6            As Long = &H3D    'long name for June
Private Const LOCALE_SMONTHNAME7            As Long = &H3E    'long name for July
Private Const LOCALE_SMONTHNAME8            As Long = &H3F    'long name for August
Private Const LOCALE_SMONTHNAME9            As Long = &H40    'long name for September
Private Const LOCALE_SMONTHNAME10           As Long = &H41    'long name for October
Private Const LOCALE_SMONTHNAME11           As Long = &H42    'long name for November
Private Const LOCALE_SMONTHNAME12           As Long = &H43    'long name for December
Private Const LOCALE_SMONTHNAME13           As Long = &H100E  'long name for 13th month (if exists)
Private Const LOCALE_SABBREVMONTHNAME1      As Long = &H44    'abbreviated name for January
Private Const LOCALE_SABBREVMONTHNAME2      As Long = &H45    'abbreviated name for February
Private Const LOCALE_SABBREVMONTHNAME3      As Long = &H46    'abbreviated name for March
Private Const LOCALE_SABBREVMONTHNAME4      As Long = &H47    'abbreviated name for April
Private Const LOCALE_SABBREVMONTHNAME5      As Long = &H48    'abbreviated name for May
Private Const LOCALE_SABBREVMONTHNAME6      As Long = &H49    'abbreviated name for June
Private Const LOCALE_SABBREVMONTHNAME7      As Long = &H4A    'abbreviated name for July
Private Const LOCALE_SABBREVMONTHNAME8      As Long = &H4B    'abbreviated name for August
Private Const LOCALE_SABBREVMONTHNAME9      As Long = &H4C    'abbreviated name for September
Private Const LOCALE_SABBREVMONTHNAME10     As Long = &H4D    'abbreviated name for October
Private Const LOCALE_SABBREVMONTHNAME11     As Long = &H4E    'abbreviated name for November
Private Const LOCALE_SABBREVMONTHNAME12     As Long = &H4F    'abbreviated name for December
Private Const LOCALE_SABBREVMONTHNAME13     As Long = &H100F  'abbreviated name for 13th month (if exists)

Private Const LOCALE_SPOSITIVESIGN          As Long = &H50    'positive sign
Private Const LOCALE_SNEGATIVESIGN          As Long = &H51    'negative sign
Private Const LOCALE_IPOSSIGNPOSN           As Long = &H52    'positive sign position
Private Const LOCALE_INEGSIGNPOSN           As Long = &H53    'negative sign position
Private Const LOCALE_IPOSSYMPRECEDES        As Long = &H54    'mon sym precedes pos amt
Private Const LOCALE_IPOSSEPBYSPACE         As Long = &H55    'mon sym sep by space from pos amt
Private Const LOCALE_INEGSYMPRECEDES        As Long = &H56    'mon sym precedes neg amt
Private Const LOCALE_INEGSEPBYSPACE         As Long = &H57    'mon sym sep by space from neg amt

'#if(WINVER >= 400)
Private Const LOCALE_FONTSIGNATURE          As Long = &H58    'font signature
Private Const LOCALE_SISO639LANGNAME        As Long = &H59    'ISO abbreviated language name
Private Const LOCALE_SISO3166CTRYNAME       As Long = &H5A    'ISO abbreviated country name
'#endif /* WINVER >= 400 */

'#if(WINVER >= 500)
Private Const LOCALE_IDEFAULTEBCDICCODEPAGE As Long = &H1012  'default ebcdic code page
Private Const LOCALE_IPAPERSIZE             As Long = &H100A  '0 = letter, 1 = a4, 2 = legal, 3 = a3
Private Const LOCALE_SENGCURRNAME           As Long = &H1007  'english name of currency
Private Const LOCALE_SNATIVECURRNAME        As Long = &H1008  'native name of currency
Private Const LOCALE_SYEARMONTH             As Long = &H1006  'year month format string
Private Const LOCALE_SSORTNAME              As Long = &H1013  'sort name
Private Const LOCALE_IDIGITSUBSTITUTION     As Long = &H1014  '0 = none, 1 = context, 2 = native digit
'#endif /* WINVER >= _WIN32_WINNT_VISTA */

'#if(WINVER >= 600)
Private Const LOCALE_SNAME                 As Long = &H5C     'locale name (with sort info) (ie: de-DE_phoneb)
Private Const LOCALE_SDURATION             As Long = &H5D     'time duration format
Private Const LOCALE_SKEYBOARDSTOINSTALL   As Long = &H5E     '(windows only) keyboard to install
Private Const LOCALE_SSHORTESTDAYNAME1     As Long = &H60     'Shortest day name for Monday
Private Const LOCALE_SSHORTESTDAYNAME2     As Long = &H61     'Shortest day name for Tuesday
Private Const LOCALE_SSHORTESTDAYNAME3     As Long = &H62     'Shortest day name for Wednesday
Private Const LOCALE_SSHORTESTDAYNAME4     As Long = &H63     'Shortest day name for Thursday
Private Const LOCALE_SSHORTESTDAYNAME5     As Long = &H64     'Shortest day name for Friday
Private Const LOCALE_SSHORTESTDAYNAME6     As Long = &H65     'Shortest day name for Saturday
Private Const LOCALE_SSHORTESTDAYNAME7     As Long = &H66     'Shortest day name for Sunday
Private Const LOCALE_SISO639LANGNAME2      As Long = &H67     '3 character ISO abbreviated language name
Private Const LOCALE_SISO3166CTRYNAME2     As Long = &H68     '3 character ISO country name
Private Const LOCALE_SNAN                  As Long = &H69     'Not a Number
Private Const LOCALE_SPOSINFINITY          As Long = &H6A     '+ Infinity
Private Const LOCALE_SNEGINFINITY          As Long = &H6B     '- Infinity
Private Const LOCALE_SSCRIPTS              As Long = &H6C     'Typical scripts in the locale
Private Const LOCALE_SPARENT               As Long = &H6D     'Fallback name for resources
Private Const LOCALE_SCONSOLEFALLBACKNAME  As Long = &H6E     'Fallback name for within the console
Private Const LOCALE_SLANGDISPLAYNAME      As Long = &H6F     'Language display name for a language

'Registry key to system locale
Private Const REG_KEY_LOCALE = "HKEY_CURRENT_USER\Control Panel\International"

'Applications should send WM_SETTINGCHANGE to all top-level windows when they make changes to system parameters.
Private Const HWND_BROADCAST                As Long = &HFFFF
Private Const WM_SETTINGCHANGE              As Long = &H1A

'-----------------------------------------------------------------------------------------------
'                                       Global Valuables
'-----------------------------------------------------------------------------------------------
Dim giOSVersion        As Long

Private Sub Form_Load()
    Dim dwVersion      As Long
    Dim dwMajorVersion As Long
    Dim dwMinorVersion As Long
    Dim dwBuild        As Long
    
    dwVersion = GetVersion()
    
    'Get the Windows version.
    dwMajorVersion = (dwVersion Mod 65536) Mod 256
    dwMinorVersion = (dwVersion Mod 65536) \ 256
    
    'Get the build number.
    If dwVersion < &H80000000 Then dwBuild = dwVersion \ 65536
    giOSVersion = 100 * dwMajorVersion + dwMinorVersion
    
    '윈도우 버젼 표시
    Me.Caption = Me.Tag & " (WINVER = " & giOSVersion & ")"
    
    '화면 갱신
    Form_Paint
End Sub

Private Sub Form_Paint()
    lblSystemLocale.Caption = GetStringValue(REG_KEY_LOCALE, "sCountry")
End Sub

Private Sub cmdSetLocale_Click(Index As Integer)
    cmdSetLocale(Index).Enabled = False
    
        Select Case giOSVersion
            Case Is >= 600
                Call SetSystemLocale_Win7(cmdSetLocale(Index).Tag)
                
            Case Else
                Call SetSystemLocale_WinXP(cmdSetLocale(Index).Tag)
        End Select
    
    Form_Paint
    cmdSetLocale(Index).Enabled = True
End Sub

Private Function GetLocaleString(ByVal Locale As Long, ByVal LCType As Long) As String
    Dim bResult(0 To 80) As Byte
    
    Select Case GetLocaleInfo(Locale, LCType, VarPtr(bResult(0)), 80)
        Case 0    '오류
            If App.LogMode = 0 Then
                Err.Raise vbObjectError, "GetLocaleLong", Hex(LCType) & "에 대한 로케일 정보를 가져올 수 없습니다"
            Else
                GetLocaleString = vbNullString
            End If
    
        Case Else '성공
            GetLocaleString = bResult
    End Select
End Function

'윈도우 XP
Private Sub SetSystemLocale_WinXP(ByVal Locale As Long)
    Call SetStringValue(REG_KEY_LOCALE, "iCalendarType", GetLocaleString(Locale, LOCALE_ICALENDARTYPE))
    Call SetStringValue(REG_KEY_LOCALE, "iCentury", GetLocaleString(Locale, LOCALE_ICENTURY))
    Call SetStringValue(REG_KEY_LOCALE, "iCountry", GetLocaleString(Locale, LOCALE_ICOUNTRY))
    Call SetStringValue(REG_KEY_LOCALE, "iCurrDigits", GetLocaleString(Locale, LOCALE_ICURRDIGITS))
    Call SetStringValue(REG_KEY_LOCALE, "iCurrency", GetLocaleString(Locale, LOCALE_ICURRENCY))
    Call SetStringValue(REG_KEY_LOCALE, "iDate", GetLocaleString(Locale, LOCALE_IDATE))
    Call SetStringValue(REG_KEY_LOCALE, "iDayLZero", GetLocaleString(Locale, LOCALE_IDAYLZERO))
    Call SetStringValue(REG_KEY_LOCALE, "iDigits", GetLocaleString(Locale, LOCALE_IDIGITS))
    Call SetStringValue(REG_KEY_LOCALE, "iFirstDayOfWeek", GetLocaleString(Locale, LOCALE_IFIRSTDAYOFWEEK))
    Call SetStringValue(REG_KEY_LOCALE, "iLZero", GetLocaleString(Locale, LOCALE_ILZERO))
    Call SetStringValue(REG_KEY_LOCALE, "iMeasure", GetLocaleString(Locale, LOCALE_IMEASURE))
    Call SetStringValue(REG_KEY_LOCALE, "iMonLZero", GetLocaleString(Locale, LOCALE_IMONLZERO))
    Call SetStringValue(REG_KEY_LOCALE, "iNegCurr", GetLocaleString(Locale, LOCALE_INEGCURR))
    Call SetStringValue(REG_KEY_LOCALE, "iNegNumber", GetLocaleString(Locale, LOCALE_INEGNUMBER))
    Call SetStringValue(REG_KEY_LOCALE, "iTime", GetLocaleString(Locale, LOCALE_ITIME))
    Call SetStringValue(REG_KEY_LOCALE, "iTimePrefix", GetLocaleString(Locale, LOCALE_ITIMEMARKPOSN))
    Call SetStringValue(REG_KEY_LOCALE, "iTLZero", GetLocaleString(Locale, LOCALE_ITLZERO))
    Call SetStringValue(REG_KEY_LOCALE, "NumShape", GetLocaleString(Locale, LOCALE_IDIGITSUBSTITUTION))
    
    Call SetStringValue(REG_KEY_LOCALE, "s1159", GetLocaleString(Locale, LOCALE_S1159))
    Call SetStringValue(REG_KEY_LOCALE, "s2359", GetLocaleString(Locale, LOCALE_S2359))
    Call SetStringValue(REG_KEY_LOCALE, "sCountry", GetLocaleString(Locale, LOCALE_SCOUNTRY))
    Call SetStringValue(REG_KEY_LOCALE, "sCurrency", GetLocaleString(Locale, LOCALE_SCURRENCY))
    Call SetStringValue(REG_KEY_LOCALE, "sDate", GetLocaleString(Locale, LOCALE_SDATE))
    Call SetStringValue(REG_KEY_LOCALE, "sDecimal", GetLocaleString(Locale, LOCALE_SDECIMAL))
    Call SetStringValue(REG_KEY_LOCALE, "sLanguage", GetLocaleString(Locale, LOCALE_SLANGUAGE))
    Call SetStringValue(REG_KEY_LOCALE, "sList", GetLocaleString(Locale, LOCALE_SLIST))
    Call SetStringValue(REG_KEY_LOCALE, "sLongDate", GetLocaleString(Locale, LOCALE_SLONGDATE))
    Call SetStringValue(REG_KEY_LOCALE, "sMonDecimalSep", GetLocaleString(Locale, LOCALE_SMONDECIMALSEP))
    Call SetStringValue(REG_KEY_LOCALE, "sMonGrouping", GetLocaleString(Locale, LOCALE_SMONGROUPING))
    Call SetStringValue(REG_KEY_LOCALE, "sMonThousandSep", GetLocaleString(Locale, LOCALE_SMONTHOUSANDSEP))
    Call SetStringValue(REG_KEY_LOCALE, "sNativeDigits", GetLocaleString(Locale, LOCALE_SNATIVEDIGITS))
    Call SetStringValue(REG_KEY_LOCALE, "sNegativeSign", GetLocaleString(Locale, LOCALE_SNEGATIVESIGN))
    Call SetStringValue(REG_KEY_LOCALE, "sPositiveSign", GetLocaleString(Locale, LOCALE_SPOSITIVESIGN))
    
    Call SetStringValue(REG_KEY_LOCALE, "sShortDate", GetLocaleString(Locale, LOCALE_SSHORTDATE))
    Call SetStringValue(REG_KEY_LOCALE, "sThousand", GetLocaleString(Locale, LOCALE_STHOUSAND))
    Call SetStringValue(REG_KEY_LOCALE, "sTime", GetLocaleString(Locale, LOCALE_STIME))
    Call SetStringValue(REG_KEY_LOCALE, "sTimeFormat", GetLocaleString(Locale, LOCALE_STIMEFORMAT))
    
    Call SetStringValue(REG_KEY_LOCALE, "Locale", Format(Hex(Locale), "00000000"))
    
    '시스템 설정을 변경했을 경우, WM_SETTINGCHANGE 메시지를 보낸다
    Call SendMessage(HWND_BROADCAST, WM_SETTINGCHANGE, 0&, ByVal 0&)
End Sub

'윈도우 7
Private Sub SetSystemLocale_Win7(ByVal Locale As Long)
    Call SetStringValue(REG_KEY_LOCALE, "iCalendarType", GetLocaleString(Locale, LOCALE_ICALENDARTYPE))
    Call SetStringValue(REG_KEY_LOCALE, "iCountry", GetLocaleString(Locale, LOCALE_ICOUNTRY))
    Call SetStringValue(REG_KEY_LOCALE, "iCurrDigits", GetLocaleString(Locale, LOCALE_ICURRDIGITS))
    Call SetStringValue(REG_KEY_LOCALE, "iCurrency", GetLocaleString(Locale, LOCALE_ICURRENCY))
    Call SetStringValue(REG_KEY_LOCALE, "iDate", GetLocaleString(Locale, LOCALE_IDATE))
    Call SetStringValue(REG_KEY_LOCALE, "iDigits", GetLocaleString(Locale, LOCALE_IDIGITS))
    Call SetStringValue(REG_KEY_LOCALE, "iFirstDayOfWeek", GetLocaleString(Locale, LOCALE_IFIRSTDAYOFWEEK))
    Call SetStringValue(REG_KEY_LOCALE, "iFirstWeekOfYear", GetLocaleString(Locale, LOCALE_IFIRSTWEEKOFYEAR))
    Call SetStringValue(REG_KEY_LOCALE, "iLZero", GetLocaleString(Locale, LOCALE_ILZERO))
    Call SetStringValue(REG_KEY_LOCALE, "iMeasure", GetLocaleString(Locale, LOCALE_IMEASURE))
    Call SetStringValue(REG_KEY_LOCALE, "iNegCurr", GetLocaleString(Locale, LOCALE_INEGCURR))
    Call SetStringValue(REG_KEY_LOCALE, "iNegNumber", GetLocaleString(Locale, LOCALE_INEGNUMBER))
    Call SetStringValue(REG_KEY_LOCALE, "iPaperSize", GetLocaleString(Locale, LOCALE_IPAPERSIZE))
    Call SetStringValue(REG_KEY_LOCALE, "iTime", GetLocaleString(Locale, LOCALE_ITIME))
    Call SetStringValue(REG_KEY_LOCALE, "iTimePrefix", GetLocaleString(Locale, LOCALE_ITIMEMARKPOSN))
    Call SetStringValue(REG_KEY_LOCALE, "iTLZero", GetLocaleString(Locale, LOCALE_ITLZERO))
    Call SetStringValue(REG_KEY_LOCALE, "NumShape", GetLocaleString(Locale, LOCALE_IDIGITSUBSTITUTION))
    
    Call SetStringValue(REG_KEY_LOCALE, "s1159", GetLocaleString(Locale, LOCALE_S1159))
    Call SetStringValue(REG_KEY_LOCALE, "s2359", GetLocaleString(Locale, LOCALE_S2359))
    Call SetStringValue(REG_KEY_LOCALE, "sCountry", GetLocaleString(Locale, LOCALE_SCOUNTRY))
    Call SetStringValue(REG_KEY_LOCALE, "sCurrency", GetLocaleString(Locale, LOCALE_SCURRENCY))
    Call SetStringValue(REG_KEY_LOCALE, "sDate", GetLocaleString(Locale, LOCALE_SDATE))
    Call SetStringValue(REG_KEY_LOCALE, "sDecimal", GetLocaleString(Locale, LOCALE_SDECIMAL))
    Call SetStringValue(REG_KEY_LOCALE, "sGrouping", GetLocaleString(Locale, LOCALE_SGROUPING))
    Call SetStringValue(REG_KEY_LOCALE, "sLanguage", GetLocaleString(Locale, LOCALE_SABBREVLANGNAME))
    Call SetStringValue(REG_KEY_LOCALE, "sList", GetLocaleString(Locale, LOCALE_SLIST))
    Call SetStringValue(REG_KEY_LOCALE, "sLongDate", GetLocaleString(Locale, LOCALE_SLONGDATE))
    Call SetStringValue(REG_KEY_LOCALE, "sMonDecimalSep", GetLocaleString(Locale, LOCALE_SMONDECIMALSEP))
    Call SetStringValue(REG_KEY_LOCALE, "sMonGrouping", GetLocaleString(Locale, LOCALE_SMONGROUPING))
    Call SetStringValue(REG_KEY_LOCALE, "sMonThousandSep", GetLocaleString(Locale, LOCALE_SMONTHOUSANDSEP))
    Call SetStringValue(REG_KEY_LOCALE, "sNativeDigits", GetLocaleString(Locale, LOCALE_SNATIVEDIGITS))
    Call SetStringValue(REG_KEY_LOCALE, "sNegativeSign", GetLocaleString(Locale, LOCALE_SNEGATIVESIGN))
    Call SetStringValue(REG_KEY_LOCALE, "sPositiveSign", GetLocaleString(Locale, LOCALE_SPOSITIVESIGN))
    Call SetStringValue(REG_KEY_LOCALE, "sShortDate", GetLocaleString(Locale, LOCALE_SSHORTDATE))
    Call SetStringValue(REG_KEY_LOCALE, "sShortTime", Replace(GetLocaleString(Locale, LOCALE_STIMEFORMAT), ":ss", ""))
    Call SetStringValue(REG_KEY_LOCALE, "sThousand", GetLocaleString(Locale, LOCALE_STHOUSAND))
    Call SetStringValue(REG_KEY_LOCALE, "sTime", GetLocaleString(Locale, LOCALE_STIME))
    Call SetStringValue(REG_KEY_LOCALE, "sTimeFormat", GetLocaleString(Locale, LOCALE_STIMEFORMAT))
    Call SetStringValue(REG_KEY_LOCALE, "sYearMonth", GetLocaleString(Locale, LOCALE_SYEARMONTH))
    
    Call SetStringValue(REG_KEY_LOCALE, "Locale", Format(Hex(Locale), "00000000"))
    Call SetStringValue(REG_KEY_LOCALE, "LocaleName", GetLocaleString(Locale, LOCALE_SNAME))
    
    '시스템 설정을 변경했을 경우, WM_SETTINGCHANGE 메시지를 보낸다
    Call SendMessage(HWND_BROADCAST, WM_SETTINGCHANGE, 0&, ByVal 0&)
End Sub
