ALTER TABLE [dbo].[Persons](
	ADD [AvatarPath] [nvarchar](200) NULL)
GO

drop table [dbo].[Tubes]
GO

CREATE TABLE [dbo].[Tubes](
	[TubeId] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Tubes] PRIMARY KEY CLUSTERED 
(
	[TubeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

drop table [dbo].[Participants]
GO

CREATE TABLE [dbo].[Participants](
	[UserId] [uniqueidentifier] NOT NULL,
	[TubeId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Participants_1] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[TubeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Participants]  WITH CHECK ADD  CONSTRAINT [FK_Participants_aspnet_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[Participants] CHECK CONSTRAINT [FK_Participants_aspnet_Users]
GO

ALTER TABLE [dbo].[Participants]  WITH CHECK ADD  CONSTRAINT [FK_Participants_Tubes] FOREIGN KEY([TubeId])
REFERENCES [dbo].[Tubes] ([TubeId])
GO

ALTER TABLE [dbo].[Participants] CHECK CONSTRAINT [FK_Participants_Tubes]
GO
 
