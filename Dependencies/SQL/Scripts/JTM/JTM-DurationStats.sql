USE [ATMS]
GO

SELECT [MsgID]
      ,[MsgDateTime]
      ,[Command]
      ,[Priority]
      ,[AtmNo]
--      ,[ATMMachineName]
--      ,[DestnFilePath]
--      ,[Stage], [ResultCode], [ResultMessage]

       ,DATEDIFF(ss,MsgDateTime, ProcessStart) AS 'Wait In Queue'
	   ,DATEDIFF(ss,FileUploadStart,FileUploadEnd) AS 'Transfer'
	   ,DATEDIFF(ss,FileUploadEnd,FileParseStart) AS 'Wait to Parse'
	   ,DATEDIFF(ss,FileParseStart,FileParseEnd) AS 'Parser'
       ,DATEDIFF(ss,ProcessStart, ProcessEnd) AS 'JTM time'
	   ,DATEDIFF(ss,MsgDateTime,ProcessEnd) AS 'Total time'
      ,[ProcessStart]
      ,[FileUploadStart]
      ,[FileUploadEnd]
      ,[FileParseStart]
      ,[FileParseEnd]
      ,[ProcessEnd]

  FROM [dbo].[JTMQueue]
  where Stage = '6' and Resultcode = '0'
  order by AtmNo, MsgID

GO


