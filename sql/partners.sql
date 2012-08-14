USE [PitchingTube]
GO

/****** Object:  Table [dbo].[Partners]    Script Date: 08/14/2012 19:00:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Partners](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[PartnerId] [uniqueidentifier] NOT NULL,
	[Contacts] [nchar](10) NOT NULL
) ON [PRIMARY]

GO


