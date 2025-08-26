@echo off
@echo.
@rem We assume the following directory structure:
@rem
@rem C:\RRDM\FilePool^|
@rem                 ^|--Atms_Journals_Txns
@rem                 ^|--Switch_Base24_Txns
@rem                 ^|--Core_Banking_T24_Txns
@rem C:\RRDM\FilePool\TestFiles^|
@rem                           ^|--Atms_Journals_Txns
@rem                           ^|--Switch_Base24_Txns
@rem                           ^|--Core_Banking_T24_Txns

@setlocal
@set Pool=C:\RRDM\FilePool
@set Exceptions=C:\RRDM\Exceptions
@set Archives=C:\RRDM\Archives
@set TestPool=C:\RRDM\FilePool\TestFiles


@set T24=Core_Banking_T24_Txns
@set Base24=Switch_Base24_Txns
@set ATMs=Atms_Journals_Txns

@set OrigT24=Banking_System
@set OrigBase24=Bank's_Switch
@set OrigATMs=ATMs

@set PoolT24=%Pool%\%T24%
@set PoolBase24=%Pool%\%Base24%
@set PoolATMs=%Pool%\%ATMs%

@set ArchT24=%Archives%\%T24%
@set ArchBase24=%Archives%\%Base24%
@set ArchATMs=%Archives%\%ATMs%


@set tT1=%TestPool%\%T24%\RRDM_NBG_17102017.txt
@set tT2=%TestPool%\%T24%\RRDM_NBG_18102017.txt
@set tT3=%TestPool%\%T24%\RRDM_NBG_19102017.txt

@set fT1=%TestPool%\%Base24%\FTODCYPR171020171200.txt
@set fT2=%TestPool%\%Base24%\FTODCYPR181020171200.txt
@set fT3=%TestPool%\%Base24%\FTODCYPR191020171200.txt

@set eJ01=%TestPool%\%ATMs%\NB0521C1_20171016_EJ_WN.126
@set eJ02=%TestPool%\%ATMs%\NB0521C1_20171017_EJ_WN.127
@set eJ03=%TestPool%\%ATMs%\NB0521C1_20171018_EJ_WN.128
@set eJ04=%TestPool%\%ATMs%\NB0525C1_20171016_EJ_WN.142
@set eJ05=%TestPool%\%ATMs%\NB0525C1_20171017_EJ_WN.143
@set eJ06=%TestPool%\%ATMs%\NB0525C1_20171018_EJ_WN.144
@set eJ07=%TestPool%\%ATMs%\NB0528C1_20171016_EJ_WN.112
@set eJ08=%TestPool%\%ATMs%\NB0528C1_20171017_EJ_WN.113
@set eJ09=%TestPool%\%ATMs%\NB0528C1_20171018_EJ_WN.114
@set eJ10=%TestPool%\%ATMs%\NB0531C1_20171016_EJ_WN.105
@set eJ11=%TestPool%\%ATMs%\NB0531C1_20171017_EJ_WN.106
@set eJ12=%TestPool%\%ATMs%\NB0531C1_20171018_EJ_WN.107
@set eJ13=%TestPool%\%ATMs%\NB0545C1_20171016_EJ_WN.104
@set eJ14=%TestPool%\%ATMs%\NB0545C1_20171017_EJ_WN.105
@set eJ15=%TestPool%\%ATMs%\NB0545C1_20171018_EJ_WN.106


@set db1=Core_Banking_T24_Txns
@set db2=Switch_Base24_Txns

:MainMenu
@echo.
@echo Select an action:
@echo.
@echo   0 - Initialize the Testing environment (clear file pool, MonitorLog, Processed Flag)
@echo   1 - Truncate table: Core_Banking_T24_Txns
@echo   2 - Truncate table: Switch_Base24_Txns
@echo   3 - Start RRDMRFM for %OrigT24% files
@echo   4 - Start RRDMRFM for %OrigBase24% files
@echo   5 - Start RRDMRFM for %OrigATMs%

@echo   6 - Select a %T24% file for import
@echo   7 - Select a %Base24% file for import
@echo   8 - Select an %ATMs% file for import
@echo   X - eXit

@set /P Action=Select an action: 
@echo.
@echo Action selected is --%Action%--
@echo.

@if %Action% equ x goto :Exit
@if %Action% equ X goto :Exit

@if %Action% equ 0 goto :InitDBs
@if %Action% equ 1 goto :TruncateNBGT2401
@if %Action% equ 2 goto :TruncateNBGBase24TLF
@if %Action% equ 3 goto :StartT2401
@if %Action% equ 4 goto :StartBase24FTOD-TLF
@if %Action% equ 5 goto :StartATMsWCR01
@if %Action% equ 6 goto :Select24_Menu
@if %Action% equ 7 goto :SelectFT_Menu
@if %Action% equ 8 goto :SelectEJ_Menu

@echo.
@echo Invalid selection...
@goto :BackToMainMenu

:InitDBs
@echo.
@echo Initialize the Testing environment (clear file pool, MonitorLog, Processed Flag)
@REM Delete files in FilePool
@del %PoolATMs%\*.* /F/Q
@del %PoolBase24%\*.* /F/Q
@del %PoolT24%\*.* /F/Q
@del %ArchATMs%\*.* /F/Q
@del %ArchBase24%\*.* /F/Q
@del %ArchT24%\*.* /F/Q
@del %Exceptions%\*.* /F/Q

@REM Reset Tables for POC
START /min RRDMRFM.exe "ResetPOC" 
@goto :BackToMainMenu

:TruncateNBGT2401
@REM Reset Tables for POC
@echo.
@echo Truncate %db1%
START /min RRDMRFM.exe "ResetPOC" "%db1%"
@goto :BackToMainMenu

:TruncateNBGBase24TLF
@REM Reset Tables for POC
@echo.
@echo Truncate %db2%
START /min RRDMRFM.exe "ResetPOC" "%db2%"
@goto :BackToMainMenu

:StartT2401
@REM  Start the monitor for FTOD-TLF
@echo.
@echo StartT2401
START /min RRDMRFM.exe "%OrigT24%" "%T24%" "ETHNCY2N"
@goto :BackToMainMenu

:StartBase24FTOD-TLF
@REM  Start the monitor for FTOD-TLF
@echo.
@echo StartBase24FTOD-TLF
START /min RRDMRFM.exe "%OrigBase24%" "%Base24%" "ETHNCY2N"
@goto :BackToMainMenu

:StartATMsWCR01
@REM  Start the monitor for Journals WCR01
@echo.
@echo StartATMsWCR01
START /min RRDMRFM.exe "%OrigATMs%" "%ATMs%" "ETHNCY2N"
@goto :BackToMainMenu

:Select24_menu
@echo.
@echo Select a file to submit for import:
@echo.
@echo   1 - %tT1%
@echo   2 - %tT2%
@echo   3 - %tT3%
@echo   X - Cancel
@set /P FNum= Select a file: 
@echo.
if %FNum% equ 1 copy /y %tT1% %PoolT24%
if %FNum% equ 2 copy /y %tT2% %PoolT24%
if %FNum% equ 3 copy /y %tT3% %PoolT24%
@if %FNum% equ x goto :BackToMainMenu
@if %FNum% equ X goto :BackToMainMenu
@goto :Select24_menu

:SelectFT_menu
@echo.
@echo Select a file to submit for import:
@echo.
@echo   1 - %FT1%
@echo   2 - %FT2%
@echo   3 - %FT3%
@echo   X - Cancel
@set /P FNum= Select a file: 
@echo.
if %FNum% equ 1 copy /y %FT1% %PoolBase24%
if %FNum% equ 2 copy /y %FT2% %PoolBase24%
if %FNum% equ 3 copy /y %FT3% %PoolBase24%
@if %FNum% equ x goto :BackToMainMenu
@if %FNum% equ X goto :BackToMainMenu
@goto :SelectFT_menu

:SelectEJ_menu
@echo.
@echo Select a file to submit for import:
@echo.
@echo  01 - %eJ01%
@echo  02 - %eJ02%
@echo  03 - %eJ03%
@echo  04 - %eJ04%
@echo  05 - %eJ05%
@echo  06 - %eJ06%
@echo  07 - %eJ07%
@echo  08 - %eJ08%
@echo  09 - %eJ09%
@echo  10 - %eJ10%
@echo  11 - %eJ11%
@echo  12 - %eJ12%
@echo  13 - %eJ13%
@echo  14 - %eJ14%
@echo  15 - %eJ15%
@echo   X - Cancel

@set /P FNum= Select a file: 
@echo.
if %FNum% equ 01 copy /y %eJ01% %PoolATMs%
if %FNum% equ 02 copy /y %eJ02% %PoolATMs%
if %FNum% equ 03 copy /y %eJ03% %PoolATMs%
if %FNum% equ 04 copy /y %eJ04% %PoolATMs%
if %FNum% equ 05 copy /y %eJ05% %PoolATMs%
if %FNum% equ 06 copy /y %eJ06% %PoolATMs%
if %FNum% equ 07 copy /y %eJ07% %PoolATMs%
if %FNum% equ 08 copy /y %eJ08% %PoolATMs%
if %FNum% equ 09 copy /y %eJ09% %PoolATMs%
if %FNum% equ 10 copy /y %eJ10% %PoolATMs%
if %FNum% equ 11 copy /y %eJ11% %PoolATMs%
if %FNum% equ 12 copy /y %eJ12% %PoolATMs%
if %FNum% equ 13 copy /y %eJ13% %PoolATMs%
if %FNum% equ 14 copy /y %eJ14% %PoolATMs%
if %FNum% equ 15 copy /y %eJ15% %PoolATMs%

@if %FNum% equ x goto :BackToMainMenu
@if %FNum% equ X goto :BackToMainMenu

@goto :SelectEJ_menu

:BackToMainMenu
@set /P dummy= Press ENTER for Main Menu...
@cls
@goto :MainMenu

:Exit