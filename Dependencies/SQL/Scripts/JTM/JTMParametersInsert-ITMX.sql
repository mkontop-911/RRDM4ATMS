USE [ATMS]
GO

DELETE FROM [dbo].[GasParameters]
      WHERE ParamId='910'

DELETE FROM [dbo].[GasParameters]
      WHERE ParamId='911'

GO


INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('910'
		    ,'1'
           ,'JTM WorkerThreads'
		   ,'JTM MaxWorkerThreads'
		   , 10
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )
GO

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('910'
		    ,'2'
           ,'JTM WorkerThreads'
		   ,'JTM FETCHRetries'
		   , 3
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )
GO

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('910'
		    ,'3'
           ,'JTM WorkerThreads'
		   ,'JTM  FETCHRetryWaitTime (sec)'
		   , 10
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )
GO

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('910'
		    ,'4'
           ,'JTM WorkerThreads'
		   ,'JTM SleepWaitEmptyThreadSlot (ms)'
		   , 1000
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )
GO

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('910'
		    ,'5'
           ,'JTM WorkerThreads'
		   ,'JTM StartWorkerThreadTimeout (ms)'
		   , 1000
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )
GO

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('910'
		    ,'6'
           ,'JTM WorkerThreads'
		   ,'JTM SleepWaitNewRequest (ms)'
		   , 2000
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )
GO

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('910'
		    ,'7'
           ,'JTM WorkerThreads'
		   ,'JTMThreadMonitorInterval (ms)'
		   , 1000
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )
GO

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('910'
		    ,'8'
           ,'JTM WorkerThreads'
		   ,'JTMThreadAbortWaitTime (ms)'
		   , 15000
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('910'
		    ,'9'
           ,'JTM WorkerThreads'
		   ,'JTM MaxThreadLifeSpan (sec)'
		   , 15000
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('911'
		    ,'1'
           ,'JTM Files'
		   ,'C:\RRDM\FilePool\ATMs'
		   , 0
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('911'
		    ,'2'
           ,'JTM Files'
		   ,'C:\RRDM\Archives\ATMs'
		   , 0
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('911'
		    ,'3'
           ,'JTM Files'
		   ,'[dbo].[stp_00_Run_Process]'
		   , 0
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )

INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('911'
		    ,'4'
           ,'JTM Files'
		   ,'JTM MaxJournalBackupsATM'
		   , 0
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )


INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('911'
		    ,'5'
           ,'JTM Files'
		   ,'C:\Tools\SysInternal\PSexec.exe'
		   , 0
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )


INSERT INTO [dbo].[GasParameters]
           ([ParamId]
           ,[OccuranceId]
		   ,[ParamNm]
		   ,[OccuranceNm]
		   ,[Amount]
           ,[OpenRecord]
		   ,[AccessLevel]
		   ,[BankId]
           ,[Operator]
           )
     VALUES
           ('911'
		    ,'6'
           ,'JTM Files'
		   ,'C:\Program Files\Advance NDC\InitEJ.exe'
		   , 0
		   , 1
		   , 3
		   ,'ITMX'
		   ,'ITMX'
		   )

GO
