USE [ATMS]
GO

DELETE FROM [dbo].[GasParameters]
      WHERE ParamId='914'

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
           ('914'
  	       ,'1'
           ,'RFMJ WorkerThreads'
		   ,'RFMJ_MaxThreadNumber'
		   , 5
		   , 1
		   , 3
		   ,'ETHNCY2N'
		   ,'ETHNCY2N'
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
           ('914'
		   ,'2'
           ,'RFMJ WorkerThreads'
		   ,'RFMJ_SleepWaitEmptyThreadSlot (ms)'
		   , 1000
		   , 1
		   , 3
		   ,'ETHNCY2N'
		   ,'ETHNCY2N'
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
           ('914'
		   ,'3'
           ,'RFMJ WorkerThreads'
		   ,'RFMJ_StartWorkerThreadTimeout (ms)'
		   , 1000
		   , 1
		   , 3
		   ,'ETHNCY2N'
		   ,'ETHNCY2N'
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
           ('914'
		   ,'4'
           ,'RFMJ WorkerThreads'
		   ,'RFMJ_ThreadMonitorInterval (ms)'
		   , 1000
		   , 1
		   , 3
		   ,'ETHNCY2N'
		   ,'ETHNCY2N'
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
           ('914'
		    ,'5'
           ,'RFMJ WorkerThreads'
		   ,'RFMJ_ThreadAbortWait (ms)'
		   , 11000
		   , 1
		   , 3
		   ,'ETHNCY2N'
		   ,'ETHNCY2N'
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
           ('914'
	       ,'6'
           ,'RFMJ WorkerThreads'
		   ,'RFMJ_MaxThreadLifeSpan (ms)'
		   , 10000
		   , 1
		   , 3
		   ,'ETHNCY2N'
		   ,'ETHNCY2N'
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
           ('914'
		   ,'7'
           ,'RFMJ WorkerThreads'
		   ,'RFMJ_RefreshInterval (seconds)'
		   , 20
		   , 1
		   , 3
		   ,'ETHNCY2N'
		   ,'ETHNCY2N'
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
           ('914'
		   ,'8'
           ,'RFMJ WorkerThreads'
		   ,'[dbo].[stp_00_RUN_PROCESS]'
		   , 0
		   , 1
		   , 3
		   ,'ETHNCY2N'
		   ,'ETHNCY2N'
		   )
 GO
 
