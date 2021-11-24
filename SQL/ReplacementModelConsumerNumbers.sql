USE [MTR_SmartNetDB]
GO

/****** Object:  UserDefinedTableType [replacement].[ReplacementModelElementsType]    Script Date: 13/09/2021 16:27:56 ******/
CREATE TYPE [replacement].[ReplacementModelConsumerNumbers] AS TABLE(
	[CommandId] [bigint] NOT NULL,
	[OldNumber] [bigint] NULL,
	[NewNumber] [bigint] NULL
)
GO


