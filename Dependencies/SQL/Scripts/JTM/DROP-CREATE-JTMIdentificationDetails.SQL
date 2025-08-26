USE [ATMS]
GO

/****** Object:  Table [dbo].[JTMIdentificationDetails]    Script Date: 20-02-2016 03:45:38 AM ******/
DROP TABLE [dbo].[JTMIdentificationDetails]
GO

/****** Object:  Table [dbo].[JTMIdentificationDetails]    Script Date: 20-02-2016 03:45:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JTMIdentificationDetails](
	[SeqNo] [int] IDENTITY(1,1) NOT NULL,
	[AtmNo] [nvarchar](20) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_AtmNo]  DEFAULT (''),
	[DateLastUpdated] [datetime] NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_MsgDateTime]  DEFAULT (getdate()),
	[UserId] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_RequestorID]  DEFAULT (''),
	[BatchID] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_BatchID]  DEFAULT (''),
	[ATMIPAddress] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_ATMIPAddress]  DEFAULT (''),
	[ATMMachineName] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_ATMMachineName]  DEFAULT (''),
	[ATMWindowsAuth] [bit] NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_ATMWindowsAuth]  DEFAULT ((0)),
	[ATMAccessID] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_ATMAccessID]  DEFAULT (''),
	[ATMAccessPassword] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_ATMAccessPassword]  DEFAULT (''),
	[TypeOfJournal] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_TypeOfJournal]  DEFAULT (''),
	[SourceFileName] [nvarchar](300) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_SourceFileName]  DEFAULT (''),
	[SourceFilePath] [nvarchar](300) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_SourceFilePath]  DEFAULT (''),
	[DestnFilePath] [nvarchar](300) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_DestnFilePath]  DEFAULT (''),
	[QueueRecId] [int] NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_QueueRecId]  DEFAULT ((0)),
	[FileUploadRequestDt] [datetime] NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_FileUploadStart]  DEFAULT ('1900-01-01'),
	[FileParseEnd] [datetime] NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_FileParseEnd]  DEFAULT ('1900-01-01'),
	[ResultCode] [int] NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_ResultCode]  DEFAULT ((0)),
	[ResultMessage] [nvarchar](128) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_ResultMessage]  DEFAULT (''),
	[Operator] [nvarchar](8) NOT NULL CONSTRAINT [DF_JTMIdentificationDetails_Operator]  DEFAULT ('')
) ON [PRIMARY]

GO


