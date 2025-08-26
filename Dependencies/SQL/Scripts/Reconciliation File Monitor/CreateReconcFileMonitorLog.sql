USE [ATMS]
GO

/****** Object:  Table [dbo].[ReconcFileAgentLog]    Script Date: 08-05-2017 07:25:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ReconcFileMonitorLog](
	[SeqNo] [int] IDENTITY(1,1) NOT NULL,
	[SystemOfOrigin] [nvarchar](50) NOT NULL,
	[SourceFileID] [nvarchar](300) NOT NULL,
	[FileName] [nvarchar](300) NOT NULL,
	[FileSize] [int] NOT NULL,
	[DateTimeReceived] [datetime] NOT NULL,
	[FileHASH] [char](64) NOT NULL,
	[LineCount] [int] NOT NULL,
	[ArchivedPath] [nvarchar](300) NOT NULL,
	[Status] [int] NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


