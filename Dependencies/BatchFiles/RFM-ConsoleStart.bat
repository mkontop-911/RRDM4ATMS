echo off
@setlocal

@SET Line0="ATMs"                     "Atms_Journals_Txns"     "BCAIEGCX"
@SET Line1="Banking_System"           "Flexcube"               "BCAIEGCX"
@SET Line2="Bank's_Switch"            "Switch_IST_Txns"        "BCAIEGCX"
@SET Line3="National_Switch"          "Egypt_123_NET"          "BCAIEGCX"
@SET Line4="Visa_International"       "VISA_CARD"              "BCAIEGCX"
@SET Line5="MasterCard_Internatinal"  "MASTER_CARD"            "BCAIEGCX"

@SET Parm0="ATMs" "Atms_Journals_Txns" "BCAIEGCX"
@SET Parm1="Banking_System" "Flexcube" "BCAIEGCX"
@SET Parm2="Bank's_Switch" "Switch_IST_Txns" "BCAIEGCX"
@SET Parm3="National_Switch" "Egypt_123_NET" "BCAIEGCX"
@SET Parm4="Visa_International" "VISA_CARD" "BCAIEGCX"
@SET Parm5="MasterCard_Internatinal" "MASTER_CARD" "BCAIEGCX"

:MainMenu
@echo.
@echo   0 - %Line0% 
@echo   1 - %Line1% 
@echo   2 - %Line2% 
@echo   3 - %Line3% 
@echo   4 - %Line4% 
@echo   5 - %Line5% 
@echo   X - Exit

@set /P Action=Select an action: 
@echo.
@echo Action selected is --%Action%--
@echo.

@if %Action% equ x goto :Exit
@if %Action% equ X goto :Exit

@if %Action% equ 0 goto :Start0
@if %Action% equ 1 goto :Start1
@if %Action% equ 2 goto :Start2
@if %Action% equ 3 goto :Start3
@if %Action% equ 4 goto :Start4
@if %Action% equ 5 goto :Start5

@echo.
@echo Invalid selection...
@goto :BackToMainMenu


:Start0
@echo.
@echo %Parm0%
@START /MIN RRDMRFMJournal_Console.exe %Parm0%
@goto :BackToMainMenu

:Start1
@echo.
@echo %Parm1%
@START /MIN RRDMRFM.exe %Parm1%
@goto :BackToMainMenu

:Start2
@echo.
@echo %Parm2%
@START /MIN RRDMRFM.exe %Parm2%
@goto :BackToMainMenu

:Start3
@echo.
@echo %Parm3%
@START /MIN RRDMRFM.exe %Parm3%
@goto :BackToMainMenu

:Start4
@echo.
@echo %Parm4%
@START /MIN RRDMRFM.exe %Parm4%
@goto :BackToMainMenu

:Start5
@echo.
@echo %Parm5%
@START /MIN RRDMRFM.exe %Parm5%
@goto :BackToMainMenu

:BackToMainMenu
@set /P dummy= Press ENTER for Main Menu...
@cls
@goto :MainMenu

:EXIT
