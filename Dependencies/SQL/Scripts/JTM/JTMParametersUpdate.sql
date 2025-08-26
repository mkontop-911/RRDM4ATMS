USE [ATMS]
GO

-- 910: JTM WorkerThreads                      
-- ----------------------

-- JTM MaxWorkerThreads
UPDATE [dbo].[GasParameters] SET [Amount]=3 WHERE (ParamId='910'and OccuranceId='1')
GO

-- JTM FETCHRetries
UPDATE [dbo].[GasParameters] SET [Amount]=3 WHERE (ParamId='910'and OccuranceId='2')
GO

-- JTM  FETCHRetryTimeout (sec)
UPDATE [dbo].[GasParameters] SET [Amount]=10 WHERE (ParamId='910'and OccuranceId='3')
GO

-- Sleep Wait Empty Thread Slot (milliseconds) 
UPDATE [dbo].[GasParameters] SET [Amount]=1000 WHERE (ParamId='910'and OccuranceId='4')
GO

-- JTM StartWorkerThreadTimeout (ms)
UPDATE [dbo].[GasParameters] SET [Amount]=1000 WHERE (ParamId='910'and OccuranceId='5')
GO

-- JTM SleepWaitNewRequest (ms)
UPDATE [dbo].[GasParameters] SET [Amount]=2000 WHERE (ParamId='910'and OccuranceId='6')
GO

-- JTM ThreadMonitorInterval (ms)
UPDATE [dbo].[GasParameters] SET [Amount]=1000 WHERE (ParamId='910'and OccuranceId='7')
GO

-- JTM ThreadAbortWaitTime (ms)
UPDATE [dbo].[GasParameters] SET [Amount]=15000 WHERE (ParamId='910'and OccuranceId='8')
GO

-- JTM MaxThreadLifeSpan (sec)     
UPDATE [dbo].[GasParameters] SET [Amount]=15000 WHERE (ParamId='910'and OccuranceId='9')
GO

-- 911: JTM Files
-- --------------

-- FilePool root directory
UPDATE [dbo].[GasParameters] SET [OccuranceNm]='C:\RRDM\FilePool\ATMs' 
WHERE (ParamId='911'and OccuranceId='1')
GO

-- Archive root directory
UPDATE [dbo].[GasParameters] SET [OccuranceNm]='C:\RRDM\Archives\ATMs' 
WHERE (ParamId='911'and OccuranceId='2')
GO

-- Stored Procedure Name
UPDATE [dbo].[GasParameters] SET [OccuranceNm]='[dbo].[stp_00_Run_Process]' 
WHERE (ParamId='911'and OccuranceId='3')
GO

-- JTM MaxJournalBackupsATM    
UPDATE [dbo].[GasParameters] SET [Amount]=5 WHERE (ParamId='911'and OccuranceId='4')
GO


-- Open Record    
UPDATE [dbo].[GasParameters] SET [OpenRecord]='true' WHERE (ParamId='910' or ParamId='911')
GO
