USE [PitchingTube]
GO
/****** Object:  Table [dbo].[Persons]    Script Date: 08/06/2012 02:05:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Persons](
	[PersonId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Skype] [nvarchar](50) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[ActivationLink] [nvarchar](150) NULL,
 CONSTRAINT [PK_Persons] PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmailTemplate]    Script Date: 08/06/2012 02:05:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailTemplate](
	[EmailTemplateID] [varchar](20) NOT NULL,
	[TemplateName] [nvarchar](100) NOT NULL,
	[Subject] [nvarchar](max) NOT NULL,
	[Template] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_EmailTemplate] PRIMARY KEY CLUSTERED 
(
	[EmailTemplateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[EmailTemplate] ([EmailTemplateID], [TemplateName], [Subject], [Template]) VALUES (N'activation', N'Account Activation Email', N'#Name#, activate your PitchingTube account', N'<b>Hello #Name#</b>,  <p>      Activate your account at PitchingTube.com by clicking on:      <br />      #ActivateLink#  </p>  <br/>  ')
INSERT [dbo].[EmailTemplate] ([EmailTemplateID], [TemplateName], [Subject], [Template]) VALUES (N'recoverpassword', N'Recover Password Email', N'Your password at PitchingTube.com', N'<div style="font-family: sans-serif">      <b>Hello #Name#</b>,      <p>          Click on the following link to change your password at PitchingTube.com:          <br />          <a href="#Url#">#Url#</a>      </p>      <br/></div>')
