USE [RRDM_AGENT]
GO

/****** Object:  Table [dbo].[ReconcFileAgentLog]    Script Date: 23-02-2016 09:32:47 AM ******/
DROP TABLE [dbo].[ReconcFileAgentLog]
GO

/****** Object:  Table [dbo].[ReconcFileAgentLog]    Script Date: 23-02-2016 09:32:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ReconcFileAgentLog](
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


