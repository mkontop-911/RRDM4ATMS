USE [RRDM_AGENT]
GO

/****** Object:  Table [dbo].[ReconcSourceFileLayout]    Script Date: 23-02-2016 09:34:30 AM ******/
DROP TABLE [dbo].[ReconcSourceFileLayout]
GO

/****** Object:  Table [dbo].[ReconcSourceFileLayout]    Script Date: 23-02-2016 09:34:30 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReconcSourceFileLayout](
	[SeqNo] [int] IDENTITY(1,1) NOT NULL,
	[LayoutID] [nvarchar](50) NOT NULL,
	[FieldID] [nvarchar](50) NOT NULL,
	[ColumnName] [nvarchar](50) NOT NULL,
	[StartPos] [int] NOT NULL,
	[Length] [int] NOT NULL,
	[FieldType] [nvarchar](50) NOT NULL
) ON [PRIMARY]

GO


