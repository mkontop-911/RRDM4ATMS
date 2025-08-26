USE [ATMS]
GO

DELETE FROM [dbo].[GasParameters]
      WHERE ParamId='913' and BankId = 'BNORPHMM'
GO

DELETE FROM [dbo].[GasParameters]
      WHERE ParamId='916' and BankId = 'BNORPHMM'
GO

-- Reconciliation File Monitor
-- ---------------------------
INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('913','1','Reconciliation File Monitor'
		   ,'RFM Sleep Time (seconds)', 10, 1, 3,'BNORPHMM','BNORPHMM' )

INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('913','2','Reconciliation File Monitor'
		   ,'BDO-ICBS:dbo.Stp_LoadHostFile', 0, 1, 3,'BNORPHMM','BNORPHMM' )

INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('913','3','Reconciliation File Monitor'
		   ,'BDO-IST:dbo.stp_LoadSwitchFile', 0, 1, 3,'BNORPHMM','BNORPHMM' )



-- MatchingBankToRRDM routine names
-- --------------------------------
INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('916','1','Reconciliation File Extraction Routines'
		   ,'Rtn_Default', 0, 1, 3,'BNORPHMM','BNORPHMM' )

INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('916','2','Reconciliation File Extraction Routines'
		   ,'Rtn_Fixed', 0, 1, 3,'BNORPHMM','BNORPHMM' )

INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('916','3','Reconciliation File Extraction Routines'
		   ,'Rtn_ICBS-Numeric', 0, 1, 3,'BNORPHMM','BNORPHMM' )

INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('916','4','Reconciliation File Extraction Routines'
		   ,'Rtn_ICBS-DateTime', 0, 1, 3,'BNORPHMM','BNORPHMM' )

INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('916','5','Reconciliation File Extraction Routines'
		   ,'Rtn_IST-Account', 0, 1, 3,'BNORPHMM','BNORPHMM' )

INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('916','6','Reconciliation File Extraction Routines'
		   ,'Rtn_IST-ResponseCode', 0, 1, 3,'BNORPHMM','BNORPHMM' )

INSERT INTO [dbo].[GasParameters] ([ParamId],[OccuranceId],[ParamNm],[OccuranceNm]
                                  ,[Amount],[OpenRecord],[AccessLevel],[BankId],[Operator])
     VALUES
           ('916','7','Reconciliation File Extraction Routines'
		   ,'Rtn_IST-FXAmtEquiv', 0, 1, 3,'BNORPHMM','BNORPHMM' )

GO