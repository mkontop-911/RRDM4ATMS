SELECT TOP 1000 [SeqNo]
      ,[SourceFileId]
      ,[SourceFieldNm]
      ,[TargetFieldType]
      ,[SourceFieldValue]
      ,[SourceFieldPositionStart]
      ,[SourceFieldPositionEnd]
      ,[TargetFieldNm]
      ,[TargetFieldValue]
      ,[RoutineValidation]
      ,[RoutineNm]
  FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields]
  where SourceFileId = 'BDO-ICBS'
  order by SourceFieldPositionStart