@echo off
@setlocal
@echo.

:Start
cls
@echo.
@cd \RRDM\RRDMExec
@echo Current Directory is: 
@cd
@echo You must run this command file "As Administrator" 
pause

SET /P BankID=Enter the BankID here: 
SET /p Proceed=The services willl be installed for BankID = %BankID%. OK to proceed? (Y/N)
if /i %Proceed%==n goto Start
if /i %Proceed%==N goto Start
if /i %Proceed%==y goto Proceed
if /i %Proceed%==Y goto Proceed
goto :Exit

:Proceed

@set user=.\RRDMService
@set pwd=RRDM$erv1ce
@set startParam=demand
@rem set startParam=delayed-auto


@set options=start= %startParam% obj= %user% password= %pwd%

@Rem @SET Line0=ATMs                Atms_Journals_Txns     %BankID% - Deprecated
@SET Line0=ATMs                     Atms_Journals_Txns
@SET Line1=Banking_System           Flexcube               %BankID%
@SET Line2=Bank's_Switch            Switch_IST_Txns        %BankID%
@SET Line3=National_Switch          Egypt_123_NET          %BankID%
@SET Line4=Visa_International       VISA_CARD              %BankID%
@SET Line5=MasterCard_Internatinal  MASTER_CARD            %BankID%
@SET Line9=RRDM Agent                                      %BankID%

@SET Orig0=ATMs
@SET Src0=Atms_Journals_Txns
@SET Parm0=ATMs Atms_Journals_Txns

@SET Orig1=Banking_System
@SET Src1=Flexcube
@SET Parm1=Banking_System Flexcube %BankID%

@SET Orig2=Bank's_Switch
@SET Src2=Bank's_Switch
@SET Parm2=Bank's_Switch Switch_IST_Txns %BankID%

@SET Orig3=National_Switch
@SET Src3=Egypt_123_NET
@SET Parm3=National_Switch Egypt_123_NET %BankID%

@SET Orig4=Visa_International
@SET Src4=VISA_CARD
@SET Parm4=Visa_International VISA_CARD %BankID%

@SET Orig5=MasterCard_Internatinal
@SET Src5=MASTER_CARD
@SET Parm5=MasterCard_Internatinal MASTER_CARD %BankID%


@SET BinaryJ=C:\RRDM\RRDMExec\RRDMRFM_Journal_Service.exe
@SET BinaryF=C:\RRDM\RRDMExec\RRDMRFMService.exe

@SET BinaryA=C:\RRDM\RRDMExec\RRDMAgent_Service.exe
@SET ParmA=%BankID%



:MainMenu
@echo.
@echo   This script must run with Administator Priviledges 
@echo   0 - Install:   %Line0% 
@echo   1 - Install:   %Line1% 
@echo   2 - Install:   %Line2% 
@echo   3 - Install:   %Line3% 
@echo   4 - Install:   %Line4% 
@echo   5 - Install:   %Line5%
@echo   9 - Install:   %Line9%
@echo. 
@echo  10 - UnInstall: %Line0% 
@echo  11 - UnInstall: %Line1% 
@echo  12 - UnInstall: %Line2% 
@echo  13 - UnInstall: %Line3% 
@echo  14 - UnInstall: %Line4% 
@echo  15 - UnInstall: %Line5% 
@echo  19 - UnInstall: %Line9% 
@echo   X - Exit

@set /P Action=Select an action: 
@echo.
@echo Action selected is --%Action%--
@echo.

@if %Action% equ x goto :Exit
@if %Action% equ X goto :Exit

@if %Action% equ 0 goto :Install_0
@if %Action% equ 1 goto :Install_1
@if %Action% equ 2 goto :Install_2
@if %Action% equ 3 goto :Install_3
@if %Action% equ 4 goto :Install_4
@if %Action% equ 5 goto :Install_5
@if %Action% equ 9 goto :Install_Agent

@if %Action% equ 10 goto :UnInstall_0
@if %Action% equ 11 goto :UnInstall_1
@if %Action% equ 12 goto :UnInstall_2
@if %Action% equ 13 goto :UnInstall_3
@if %Action% equ 14 goto :UnInstall_4
@if %Action% equ 15 goto :UnInstall_5
@if %Action% equ 19 goto :UnInstall_Agent

@echo.
@echo Invalid selection...
@goto :BackToMainMenu


:Install_0
@echo.
@SET Orig=%Orig0%
@SET Parm=%Parm0%
@SET Src=%Src0%
@SET binary=%BinaryJ%
@goto :DoInstall

:Install_1
@echo.
@SET Orig=%Orig1%
@SET Parm=%Parm1%
@SET Src=%Src1%
@SET binary=%BinaryF%
@goto :DoInstall

:Install_2
@echo.
@SET Orig=%Orig2%
@SET Parm=%Parm2%
@SET Src=%Src2%
@SET binary=%BinaryF%
@goto :DoInstall

:Install_3
@echo.
@SET Orig=%Orig3%
@SET Parm=%Parm3%
@SET Src=%Src3%
@SET binary=%BinaryF%
@goto :DoInstall

:Install_4
@echo.
@SET Orig=%Orig4%
@SET ParmA=%Parm4%
@SET Src=%Src4%
@SET binary=%BinaryF%
@goto :DoInstall

:Install_5
@echo.
@SET Orig=%Orig5%
@SET Parm=%Parm5%
@SET Src=%Src5%
@SET binary=%BinaryF%
@goto :DoInstall

:Install_Agent
@echo.
@SET Parm=%ParmA%
@SET binary=%BinaryA%
@echo Installing Service "RRDM Agent" binpath= "%binary% %Parm%"
@sc create "RRDM Agent" binpath= "%binary% %Parm%" %options%
@sc description "RRDM Agent" "RRDM Agent service for BankID '%Parm%'"
@sc start "RRDM Agent"
@timeout 5 
@@sc qc "RRDM Agent"

@echo.
@goto :BackToMainMenu


:DoInstall
@echo Installing Service "RRDM RFM %Orig%" binpath= "%binary% %Parm%"
@sc create "RRDM RFM %Orig%" binpath= "%binary% %Parm%" %options%
@rem @sc description "RRDM RFM %Orig%" "RRDM Reconciliation File Monitor for BankID '%BankID%' 'and source files of type' %Src%' '%Orig%'"
@sc description "RRDM RFM %Orig%" "RRDM Reconciliation File Monitor for source files of type' %Src%' '%Orig%'"
@rem @sc start "RRDM RFM %Orig%"
@timeout 5 
@@sc qc "RRDM RFM %Orig%"
@echo.
@goto :BackToMainMenu



:UnInstall_0
@SET Orig=%Orig0%
goto :DoUninstall

:UnInstall_1
@SET Orig=%Orig1%
goto :DoUninstall

:UnInstall_2
@SET Orig=%Orig2%
goto :DoUninstall

:UnInstall_3
@SET Orig=%Orig3%
goto :DoUninstall

:UnInstall_4
@SET Orig=%Orig4%
goto :DoUninstall

:UnInstall_5
@SET Orig=%Orig5%
goto :DoUninstall


:DoUnInstall
@echo Removing service: "RRDM RFM %Orig%"
@sc stop "RRDM RFM %Orig%"
@timeout 5 
@sc delete "RRDM RFM %Orig%"
@echo.
goto :BackToMainMenu

:UnInstall_Agent
@echo Removing service: "RRDM Agent"
@sc stop "RRDM Agent"
@timeout 5 
@sc delete "RRDM Agent"
@echo.
goto :BackToMainMenu

:BackToMainMenu
@set /P dummy= Press ENTER for Main Menu...
@cls
@goto :MainMenu

:EXIT
