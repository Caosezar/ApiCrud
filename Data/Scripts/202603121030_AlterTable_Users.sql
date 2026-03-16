USE [ApiCrudDB]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 12/03/2026 10:30:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[BirthDate] [date] NULL,
	[IsActive] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[PasswordHash] [nvarchar](500) NOT NULL,
	[Username] [nvarchar](50) NULL,
	[RoleId] [tinyint] NOT NULL,
	[LastLogin] [datetime] NULL,
	[FailedLoginAttempts] [tinyint] NULL,
	[LockoutEnd] [datetime] NULL,
	[RefreshToken] [nvarchar](500) NULL,
	[RefreshTokenExpiryTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ('') FOR [PasswordHash]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ((2)) FOR [RoleId]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [FailedLoginAttempts]
GO

ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [CHK_BirthDate] CHECK  (([BirthDate]<=getdate()))
GO

ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [CHK_BirthDate]
GO

ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [CHK_Email] CHECK  (([Email] like '%_@__%.__%'))
GO

ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [CHK_Email]
GO

