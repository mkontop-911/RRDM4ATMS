USE [RRDM_Reconciliation_ITMX]
GO

/****** Object:  Table [dbo].[NBGBase24TLF_RAW]    Script Date: 29-10-2017 10:29:35 AM ******/
DROP TABLE [dbo].[NBGBase24TLF_RAW]
GO

/****** Object:  Table [dbo].[NBGBase24TLF_RAW]    Script Date: 29-10-2017 10:29:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NBGBase24TLF_RAW](
	[SeqNo] [int] NOT NULL,
	[Origin] [nvarchar](50) NOT NULL,
	[OriginFileName] [nvarchar](300) NOT NULL,
	[OriginalRecordId] [int] NOT NULL,
	[TransDate] [date] NOT NULL CONSTRAINT [DF_NBGBase24TLF_RAW_RecDate]  DEFAULT ('01-01-1900'),
	[TransTraceNumber] [int] NOT NULL CONSTRAINT [DF_NBGBase24TLF_RAW_RecTraceNumber]  DEFAULT ((0)),
	[W_DATE_EPEX] [nvarchar](6) NOT NULL,
	[W_REC] [nvarchar](4) NOT NULL,
	[W_TRAN_DATE] [nvarchar](10) NOT NULL,
	[W_TRAN_TIME] [nvarchar](8) NOT NULL,
	[W_PAN] [nvarchar](19) NOT NULL,
	[W_TERM_ID] [nvarchar](16) NOT NULL,
	[W_FROM_ACCT] [nvarchar](19) NOT NULL,
	[W_TO_ACCT] [nvarchar](19) NOT NULL,
	[W_POSTDAT] [nvarchar](6) NOT NULL,
	[W_T_CDE] [nvarchar](2) NOT NULL,
	[W_AMT_1] [nvarchar](10) NOT NULL,
	[W_CODE] [nvarchar](4) NOT NULL,
	[W_SEQ_NUM] [nvarchar](12) NOT NULL,
	[W_AUTH_ID] [nvarchar](6) NOT NULL,
	[FILLER] [nvarchar](2) NOT NULL,
	[W_COUNTRY] [nvarchar](2) NOT NULL,
	[W_CURRENCY] [nvarchar](3) NOT NULL,
	[W_TERMINAL_CAPABILITY] [nvarchar](1) NOT NULL,
	[W_CARD_TYPE] [nvarchar](1) NOT NULL,
	[W_DATE_EPEX2] [nvarchar](54) NOT NULL,
	[Operator] [nvarchar](8) NOT NULL
) ON [PRIMARY]

GO


