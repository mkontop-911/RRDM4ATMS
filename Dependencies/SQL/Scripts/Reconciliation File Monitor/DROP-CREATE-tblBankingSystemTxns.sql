USE [RRDM_Reconciliation_ITMX]
GO

/****** Object:  Table [dbo].[IntblBankingSystemTxns]    Script Date: 09-06-2017 01:49:23 PM ******/
DROP TABLE [dbo].[IntblBankingSystemTxns]
GO

/****** Object:  Table [dbo].[IntblBankingSystemTxns]    Script Date: 09-06-2017 01:49:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[IntblBankingSystemTxns](
	[SeqNo] [int] IDENTITY(1,1) NOT NULL,
	[OriginFileName] [nvarchar](300) NOT NULL CONSTRAINT [DF_tblBankingSystem_OriginFileName]  DEFAULT ('OriginFLNm'),
	[OriginalRecordId] [int] NOT NULL CONSTRAINT [DF_tblBankingSystem_OriginalRecordId]  DEFAULT ((0)),
	[MatchingCateg] [nvarchar](50) NOT NULL CONSTRAINT [DF_tblBankingSystem_RMCateg]  DEFAULT (''),
	[Origin] [nvarchar](50) NOT NULL CONSTRAINT [DF_tblBankingSystem_Origin]  DEFAULT (''),
	[TransTypeAtOrigin] [nvarchar](50) NOT NULL CONSTRAINT [DF_tblBankingSystem_TransTypeAtOrigin]  DEFAULT ('TransactionType'),
	[TerminalId] [nvarchar](20) NOT NULL CONSTRAINT [DF_tblBankingSystem_TerminalId]  DEFAULT (''),
	[TransType] [int] NOT NULL CONSTRAINT [DF_tblBankingSystem_TransType]  DEFAULT ((11)),
	[TransDescr] [nvarchar](50) NOT NULL CONSTRAINT [DF_tblBankingSystem_TransDescr]  DEFAULT (''),
	[CardNumber] [nvarchar](20) NOT NULL CONSTRAINT [DF_tblBankingSystem_CardNumber]  DEFAULT (''),
	[AccNo] [nvarchar](30) NOT NULL CONSTRAINT [DF_tblBankingSystem_AccNumber]  DEFAULT (''),
	[TransCurr] [nvarchar](3) NOT NULL CONSTRAINT [DF_tblBankingSystem_TransCurr]  DEFAULT (''),
	[TransAmt] [decimal](18, 2) NOT NULL CONSTRAINT [DF_tblBankingSystem_TransAmount]  DEFAULT ((0)),
	[AmtFileBToFileC] [decimal](18, 2) NOT NULL CONSTRAINT [DF_tblBankingSystem_AmtFileBToFileC]  DEFAULT ((0)),
	[TransDate] [datetime] NOT NULL CONSTRAINT [DF_tblBankingSystem_TransDate]  DEFAULT ('1900-01-01'),
	[TraceNo] [int] NOT NULL CONSTRAINT [DF_tblBankingSystem_AtmTraceNo]  DEFAULT ((0)),
	[RRNumber] [int] NOT NULL CONSTRAINT [DF_tblBankingSystem_RRNumber]  DEFAULT ((0)),
	[ResponseCode] [nvarchar](50) NOT NULL CONSTRAINT [DF_tblBankingSystem_ResponseCode]  DEFAULT (''),
	[T24RefNumber] [nvarchar](60) NOT NULL CONSTRAINT [DF_tblBankingSystem_T24RefNumber]  DEFAULT ((1)),
	[Processed] [bit] NOT NULL CONSTRAINT [DF_tblBankingSystem_OpenRecord]  DEFAULT ((0)),
	[ProcessedAtRMCycle] [int] NOT NULL CONSTRAINT [DF_tblBankingSystem_RMCycle]  DEFAULT ((0)),
	[Operator] [nvarchar](8) NOT NULL CONSTRAINT [DF_tblBankingSystem_Operator]  DEFAULT (''),
 CONSTRAINT [PK_tblBankingSystem] PRIMARY KEY CLUSTERED 
(
	[SeqNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


