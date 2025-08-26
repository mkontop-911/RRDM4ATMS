USE [ATMS]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_Operator]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_CmdExecFinished]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_CmdDateTime]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_ReqResultMessage]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_ReqResultCode]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_ReqStatusCode]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_Priority]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_ServiceName]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_ServiceId]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_Command]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_RequestorMachine]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_RequestorID]
GO

ALTER TABLE [dbo].[AgentQueue] DROP CONSTRAINT [DF_AgentQueue_ReqDateTime]
GO

/****** Object:  Table [dbo].[AgentQueue]    Script Date: 03/07/2019 9:32:28 μ.μ. ******/
DROP TABLE [dbo].[AgentQueue]
GO

/****** Object:  Table [dbo].[AgentQueue]    Script Date: 03/07/2019 9:32:28 μ.μ. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AgentQueue](
	[ReqID] [int] IDENTITY(1,1) NOT NULL,
	[ReqDateTime] [datetime] NOT NULL,
	[RequestorID] [nvarchar](50) NOT NULL,
	[RequestorMachine] [nvarchar](50) NOT NULL,
	[Command] [nvarchar](30) NOT NULL,
	[ServiceId] [nvarchar](100) NOT NULL,
	[ServiceName] [nvarchar](100) NOT NULL,
	[Priority] [int] NOT NULL,
	[ReqStatusCode] [int] NOT NULL,
	[CmdStatusCode] [int] NOT NULL,
	[CmdStatusMessage] [nvarchar](1024) NOT NULL,
	[CmdExecStarted] [datetime] NOT NULL,
	[CmdExecFinished] [datetime] NOT NULL,
	[Operator] [nvarchar](8) NOT NULL,
 CONSTRAINT [PK_AgentQueueReqID] PRIMARY KEY CLUSTERED 
(
	[ReqID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_ReqDateTime]  DEFAULT (getdate()) FOR [ReqDateTime]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_RequestorID]  DEFAULT ('') FOR [RequestorID]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_RequestorMachine]  DEFAULT ('') FOR [RequestorMachine]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_Command]  DEFAULT ('') FOR [Command]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_ServiceId]  DEFAULT ('') FOR [ServiceId]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_ServiceName]  DEFAULT ('') FOR [ServiceName]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_Priority]  DEFAULT ((2)) FOR [Priority]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_ReqStatusCode]  DEFAULT ((0)) FOR [ReqStatusCode]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_ReqResultCode]  DEFAULT ((-1)) FOR [CmdStatusCode]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_ReqResultMessage]  DEFAULT ('') FOR [CmdStatusMessage]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_CmdDateTime]  DEFAULT ('1900-01-01') FOR [CmdExecStarted]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_CmdExecFinished]  DEFAULT ('1900-01-01') FOR [CmdExecFinished]
GO

ALTER TABLE [dbo].[AgentQueue] ADD  CONSTRAINT [DF_AgentQueue_Operator]  DEFAULT ('') FOR [Operator]
GO


