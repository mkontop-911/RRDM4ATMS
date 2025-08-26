@echo off

@REM Delete files in FilePool
@del C:\RRDM\FilePool\T24\*.* /F/Q
@del C:\RRDM\FilePool\BANCNET\*.* /F/Q
@del C:\RRDM\FilePool\NARADA\*.* /F/Q

@REM Delete files in Archives
@del C:\RRDM\Archives\T24\*.* /F/Q
@del C:\RRDM\Archives\BANCNET\*.* /F/Q
@del C:\RRDM\Archives\NARADA\*.* /F/Q

@REM Reset Tables for POC
START /min RRDMFileAgent.exe "ResetPOC"

@REM  Start the three monitors
START /min RRDMFileAgent.exe "T24" "T-24 File"
START /min RRDMFileAgent.exe "NARADA" "Switch File"
START /min RRDMFileAgent.exe "BANCNET" "BancNet File"
