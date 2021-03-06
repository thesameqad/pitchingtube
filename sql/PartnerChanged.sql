USE [PitchingTube]
GO
/****** Object:  Table [dbo].[Partners]    Script Date: 08/24/2012 01:38:26 ******/
DROP TABLE [dbo].[Partners]
GO
/****** Object:  Table [dbo].[Partners]    Script Date: 08/24/2012 01:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Partners](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[PartnerId] [uniqueidentifier] NOT NULL,
	[Contacts] [nchar](10) NULL,
	[BeginPitchTime] [datetime] NULL,
 CONSTRAINT [PK_Partners] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
