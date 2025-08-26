USE [RRDM_Reconciliation_ITMX]
GO

/****** Object:  Table [dbo].[NBGBase24TLF]    Script Date: 29-10-2017 10:31:29 AM ******/
DROP TABLE [dbo].[NBGBase24TLF]
GO

/****** Object:  Table [dbo].[NBGBase24TLF]    Script Date: 29-10-2017 10:31:29 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NBGBase24TLF](
	[SeqNo] [int] NOT NULL,
	[OriginFileName] [nvarchar](300) NOT NULL,
	[OriginalRecordId] [int] NOT NULL,
	[MatchingCateg] [nvarchar](50) NOT NULL,
	[Origin] [nvarchar](50) NOT NULL,
	[TransTypeAtOrigin] [nvarchar](50) NOT NULL,
	[TerminalId] [nvarchar](20) NOT NULL,
	[TransType] [int] NOT NULL,
	[TransDescr] [nvarchar](50) NOT NULL,
	[CardNumber] [nvarchar](20) NOT NULL,
	[AccNo] [nvarchar](30) NOT NULL,
	[TransCurr] [nvarchar](3) NOT NULL,
	[TransAmt] [decimal](18, 2) NOT NULL,
	[AmtFileBToFileC] [decimal](18, 2) NOT NULL,
	[TransDate] [datetime] NOT NULL,
	[TraceNo] [int] NOT NULL,
	[RRNumber] [int] NOT NULL,
	[ResponseCode] [nvarchar](50) NOT NULL,
	[T24RefNumber] [nvarchar](60) NOT NULL,
	[Processed] [bit] NOT NULL,
	[ProcessedAtRMCycle] [int] NOT NULL,
	[Mask] [nvarchar](10) NOT NULL CONSTRAINT [DF_NBGBase24TLF_Mask]  DEFAULT (''),
	[ItHasException] [bit] NOT NULL CONSTRAINT [DF_NBGBase24TLF_ItHasException]  DEFAULT ((0)),
	[UniqueRecordId] [int] NOT NULL CONSTRAINT [DF_NBGBase24TLF_UniqueRecordId]  DEFAULT ((0)),
	[Operator] [nvarchar](8) NOT NULL
) ON [PRIMARY]

GO


