USE [RRDM_Reconciliation_ITMX]
GO

/****** Object:  Table [dbo].[IntblBankSwitchTxns]    Script Date: 09-06-2017 11:45:45 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[IntblBankSwitchTxns](
	[SeqNo] [int] IDENTITY(1,1) NOT NULL,
	[OriginFileName] [nvarchar](300) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_OriginFileName]  DEFAULT ('OriginFLNm'),
	[OriginalRecordId] [int] NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_OriginalRecordId]  DEFAULT ((0)),
	[MatchingCateg] [nvarchar](50) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_MatchingCateg]  DEFAULT (''),
	[Origin] [nvarchar](50) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_Origin]  DEFAULT (''),
	[TransTypeAtOrigin] [nvarchar](50) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_TransTypeAtOrigin]  DEFAULT ('TransactionType'),
	[TerminalId] [nvarchar](20) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_TerminalId]  DEFAULT (''),
	[TransType] [int] NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_TransType]  DEFAULT ((11)),
	[TransDescr] [nvarchar](50) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_TransDescr]  DEFAULT (''),
	[CardNumber] [nvarchar](20) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_CardNumber]  DEFAULT (''),
	[AccNo] [nvarchar](30) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_AccNo]  DEFAULT (''),
	[TransCurr] [nvarchar](3) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_TransCurr]  DEFAULT (''),
	[TransAmt] [decimal](18, 2) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_TranAmt]  DEFAULT ((0)),
	[AmtFileBToFileC] [decimal](18, 2) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_AmtFileBToFileC]  DEFAULT ((0)),
	[TransDate] [datetime] NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_TransDate]  DEFAULT ('1900-01-01'),
	[TraceNo] [int] NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_TraceNo]  DEFAULT ((0)),
	[RRNumber] [int] NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_RRNumber]  DEFAULT ((0)),
	[ResponseCode] [nvarchar](50) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_ResponseCode]  DEFAULT (''),
	[T24RefNumber] [nvarchar](60) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_T24RefNumber]  DEFAULT ((1)),
	[Processed] [bit] NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_Processed]  DEFAULT ((0)),
	[ProcessedAtRMCycle] [int] NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_ProcessedAtRMCycle]  DEFAULT ((0)),
	[Operator] [nvarchar](8) NOT NULL CONSTRAINT [DF_IntblBankSwitchTxns_Operator]  DEFAULT (''),
 CONSTRAINT [PK_IntblBankSwitchTxns] PRIMARY KEY CLUSTERED 
(
	[SeqNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


