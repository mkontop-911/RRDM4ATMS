USE [ATMS]
GO

/****** Object:  Table [dbo].[JTMQueue]    Script Date: 20-02-2016 03:43:26 AM ******/
DROP TABLE [dbo].[JTMQueue]
GO

/****** Object:  Table [dbo].[JTMQueue]    Script Date: 20-02-2016 03:43:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[JTMQueue](
	[MsgID] [int] IDENTITY(1,1) NOT NULL,
	[MsgDateTime] [datetime] NOT NULL CONSTRAINT [DF_JTMQueue_MsgDateTime]  DEFAULT (getdate()),
	[RequestorID] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMQueue_RequestorID]  DEFAULT (''),
	[RequestorMachine] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMQueue_RequestorMachine]  DEFAULT (''),
	[Command] [nvarchar](30) NOT NULL CONSTRAINT [DF_JTMQueue_Command]  DEFAULT (''),
	[Priority] [int] NOT NULL CONSTRAINT [DF_JTMQueue_Priority]  DEFAULT ((2)),
	[BatchID] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMQueue_BatchID]  DEFAULT (''),
	[AtmNo] [nvarchar](20) NOT NULL CONSTRAINT [DF_JTMQueue_AtmNo]  DEFAULT (''),
	[BankID] [nvarchar](8) NOT NULL CONSTRAINT [DF_JTMQueue_BankID]  DEFAULT (''),
	[BranchNo] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMQueue_BranchNo]  DEFAULT (''),
	[ATMIPAddress] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMQueue_ATMIPAddress]  DEFAULT (''),
	[ATMMachineName] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMQueue_ATMMachineName]  DEFAULT (''),
	[ATMWindowsAuth] [bit] NOT NULL CONSTRAINT [DF_JTMQueue_ATMWindowsAuth]  DEFAULT ((0)),
	[ATMAccessID] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMQueue_ATMAccessID]  DEFAULT (''),
	[ATMAccessPassword] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMQueue_ATMAccessPassword]  DEFAULT (''),
	[TypeOfJournal] [nvarchar](50) NOT NULL CONSTRAINT [DF_JTMQueue_TypeOfJournal]  DEFAULT (''),
	[SourceFileName] [nvarchar](300) NOT NULL CONSTRAINT [DF_JTMQueue_SourceFileName]  DEFAULT (''),
	[SourceFilePath] [nvarchar](300) NOT NULL CONSTRAINT [DF_JTMQueue_SourceFilePath]  DEFAULT (''),
	[DestnFileName] [nvarchar](300) NOT NULL CONSTRAINT [DF_JTMQueue_DestnFileName]  DEFAULT (''),
	[DestnFilePath] [nvarchar](300) NOT NULL CONSTRAINT [DF_JTMQueue_DestnFilePath]  DEFAULT (''),
	[DestnFileHASH] [char](64) NOT NULL CONSTRAINT [DF_JTMQueue_DestnFileHASH]  DEFAULT (''),
	[Stage] [int] NOT NULL CONSTRAINT [DF_JTMQueue_Stage]  DEFAULT ((0)),
	[ResultCode] [int] NOT NULL CONSTRAINT [DF_JTMQueue_ResultCode]  DEFAULT ((0)),
	[ResultMessage] [nvarchar](512) NOT NULL CONSTRAINT [DF_JTMQueue_ResultMessage]  DEFAULT (''),
	[ProcessStart] [datetime] NOT NULL CONSTRAINT [DF_JTMQueue_ProcessStart]  DEFAULT ('1900-01-01'),
	[FileUploadStart] [datetime] NOT NULL CONSTRAINT [DF_JTMQueue_FileUploadStart]  DEFAULT ('1900-01-01'),
	[FileUploadEnd] [datetime] NOT NULL CONSTRAINT [DF_JTMQueue_FileUploadEnd]  DEFAULT ('1900-01-01'),
	[FileParseStart] [datetime] NOT NULL CONSTRAINT [DF_JTMQueue_FileParseStart]  DEFAULT ('1900-01-01'),
	[FileParseEnd] [datetime] NOT NULL CONSTRAINT [DF_JTMQueue_FileParseEnd]  DEFAULT ('1900-01-01'),
	[ProcessEnd] [datetime] NOT NULL CONSTRAINT [DF_JTMQueue_ProcessEnd]  DEFAULT ('1900-01-01'),
	[Operator] [nvarchar](8) NOT NULL CONSTRAINT [DF_JTMQueue_Operator]  DEFAULT (''),
 CONSTRAINT [PK_JTMQueue] PRIMARY KEY CLUSTERED 
(
	[MsgID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


