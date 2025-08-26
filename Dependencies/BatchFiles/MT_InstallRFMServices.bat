@echo off
@echo.
@rem We assume the service will run on this machine
@rem obj= ".\Alex" password= "..."

@set user=.\RRDMService
@set pwd=RRDM$erv1ce
@set startParam=delayed-auto

@set options=start= %startParam% obj= %user% password= %pwd%


@rem "ATMs" "Atms_Journals_Txns" "ETHNCY2N"
@rem "Bank's_Switch" "Switch_Base24_Txns" "ETHNCY2N"
@rem "Banking_System" "Core_Banking_T24_Txns" "ETHNCY2N"

@set T24=Core_Banking_T24_Txns
@set Base24=Switch_Base24_Txns
@set ATMs=Atms_Journals_Txns

@set OrigT24=Banking_System
@set OrigBase24=Bank's_Switch
@set OrigATMs=ATMs

@C:
@cd \RRDM\RRDMExec
@echo Current Directory is: 
@cd
@echo You must run this command file "As Administrator" 
pause

:MainMenu
cls
@echo.
@echo Select an action:
@echo.
@echo   0 - Install The RRDM Solutions Event Source in EventLog
@echo   1 - Install RRDMRFM service for '%OrigATMs%'::'%ATMs%' type of source files
@echo   2 - Install RRDMRFM service for '%OrigT24%'::'%T24%' type of source files
@echo   3 - Install RRDMRFM service for '%OrigBase24%'::'%Base24%' type of source files
@echo  11 - Uninstall RRDMRFM service for '%OrigATMs%'::'%ATMs%' type of source files
@echo  12 - Uninstall RRDMRFM service for '%OrigT24%'::'%T24%' type of source files
@echo  13 - Uninstall RRDMRFM service for '%OrigBase24%'::'%Base24%' type of source files
@echo   X - eXit

@set /P Action=Select an action: 
@echo.
@echo Action selected is --%Action%--
@echo.

@if %Action% equ x goto :Exit
@if %Action% equ X goto :Exit

@if %Action% equ 0 goto :EventLog
@if %Action% equ 1 goto :ATMs_WCR01
@if %Action% equ 2 goto :T24_T2401
@if %Action% equ 3 goto :Base24_FTOD-TLF
@if %Action% equ 11 goto :Uninstall_ATMs_WCR01
@if %Action% equ 12 goto :Uninstall_T24_T2401
@if %Action% equ 13 goto :Uninstall_Base24_FTOD-TLF

@echo.
@echo Invalid selection...
@goto :BackToMainMenu

:UserCredentialsReminder
@echo.
@echo Please run "Administrative Tools, Services" to:
@echo   1. Configure the User Account for this service
@echo   2. Configure the service to start automatically
@echo.
goto :BackToMainMenu

:EventLog
@echo.
@echo  Starting RRDMEventSourceInstaller.exe
@RRDMEventSourceInstaller.exe
@echo.
goto :BackToMainMenu


:ATMs_WCR01
@echo sc create "RRDM RFM %OrigATMs%" binpath= "C:\RRDM\RRDMExec\RRDMRFM_Journal_Service.exe %OrigATMs% %ATMs% ETHNCY2N" %options%
sc create "RRDM RFM %OrigATMs%" binpath= "C:\RRDM\RRDMExec\RRDMRFM_Journal_Service.exe %OrigATMs% %ATMs% ETHNCY2N" %options%
@sc description "RRDM RFM %OrigATMs%" "RRDM Reconciliation File Monitor for '%OrigATMs%' source files of type '%ATMs%'"
@sc start "RRDM RFM %OrigATMs%"
@timeout 5 
@sc qc "RRDM RFM %OrigATMs%"
@echo.
goto :UserCredentialsReminder

:T24_T2401
sc create "RRDM RFM %OrigT24%" binpath= "C:\RRDM\RRDMExec\RRDMRFMService.exe %OrigT24% %T24% ETHNCY2N" %options%
@sc description "RRDM RFM %OrigT24%" "RRDM Reconciliation File Monitor for '%OrigT24%' source files of type '%T24%'"
@sc start "RRDM RFM %OrigT24%"
@timeout 5 
@sc qc "RRDM RFM %OrigT24%"
@echo.
goto :UserCredentialsReminder

:Base24_FTOD-TLF
sc create "RRDM RFM %OrigBase24%" binpath= "C:\RRDM\RRDMExec\RRDMRFMService.exe %OrigBase24% %Base24% ETHNCY2N" %options%
@sc description "RRDM RFM %OrigBase24%" "RRDM Reconciliation File Monitor for '%OrigBase24%' source files of type '%Base24%'"
@sc start "RRDM RFM %OrigBase24%"
@timeout 5 
@sc qc "RRDM RFM %OrigBase24%"
@echo.
goto :UserCredentialsReminder

:Uninstall_ATMs_WCR01
echo Removing service: "RRDM RFM %OrigATMs%"
sc stop "RRDM RFM %OrigATMs%"
@timeout 5 
sc delete "RRDM RFM %OrigATMs%"
@echo.
goto :BackToMainMenu

:Uninstall_T24_T2401
echo Removing service: "RRDM RFM %OrigT24%"
sc stop "RRDM RFM %OrigT24%"
@timeout 5 
sc delete "RRDM RFM %OrigT24%"
@echo.
goto :BackToMainMenu

:Uninstall_Base24_FTOD-TLF
echo Removing service: "RRDM RFM %OrigBase24%"
sc stop "RRDM RFM %OrigBase24%"
@timeout 5 
sc delete "RRDM RFM %OrigBase24%"
@echo.
goto :BackToMainMenu

:BackToMainMenu
@set /P dummy= Press ENTER for Main Menu...
@cls
@goto :MainMenu

:Exit