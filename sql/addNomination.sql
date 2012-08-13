USE [PitchingTube]
GO

/****** Object:  Table [dbo].[Nominations]    Script Date: 08/13/2012 00:48:08 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Nominations]') AND type in (N'U'))
DROP TABLE [dbo].[Nominations]
GO

USE [PitchingTube]
GO

/****** Object:  Table [dbo].[Nominations]    Script Date: 08/13/2012 00:48:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Nominations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TubeId] [int] NOT NULL,
	[InvestorId] [uniqueidentifier] NOT NULL,
	[EnterepreneurId] [uniqueidentifier] NOT NULL,
	[Rating] [int] NOT NULL,
	[Panding] [int] NOT NULL,
 CONSTRAINT [PK_Nominations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

