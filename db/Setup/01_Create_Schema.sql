USE [iidentifii-web-forum];

IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'user_category')
BEGIN
	CREATE TABLE [user_category] (
	  [user_category_id] INT IDENTITY (1, 1) PRIMARY KEY CLUSTERED NOT NULL,
	  [user_category_name] NVARCHAR(20) NOT NULL
	)
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'user')
BEGIN
  CREATE TABLE [user] (
    [user_id] INT IDENTITY (1, 1) PRIMARY KEY CLUSTERED NOT NULL,
    [user_guid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [user_category_id] INT NOT NULL,
    [first_name] NVARCHAR(500) NOT NULL,
    [last_name] NVARCHAR(500) NOT NULL,
    [email] NVARCHAR(500) NOT NULL,
    [username] NVARCHAR(500) NOT NULL,
    [password] NVARCHAR(500) NOT NULL,
    [date_created] DATETIME NOT NULL DEFAULT GETUTCDATE()
  )

  ALTER TABLE [user] ADD FOREIGN KEY ([user_category_id]) REFERENCES [user_category] ([user_category_id])
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'tag')
BEGIN
  CREATE TABLE [tag] (
    [tag_id] INT IDENTITY (1, 1) PRIMARY KEY CLUSTERED NOT NULL,
    [tag_name] NVARCHAR(100) NOT NULL
  )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'post')
BEGIN
  CREATE TABLE [post] (
    [post_id] INT IDENTITY (1, 1) PRIMARY KEY CLUSTERED NOT NULL,
    [post_guid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [title] NVARCHAR(200) NOT NULL,
    [content] NVARCHAR(1000) NOT NULL,
    [user_id] INT NOT NULL,
    [tag_id] INT NULL, -- intentionally null
    [date_created] DATETIME NOT NULL DEFAULT GETUTCDATE()
  )

  ALTER TABLE [post] ADD FOREIGN KEY ([user_id]) REFERENCES [user] ([user_id])

  ALTER TABLE [post] ADD FOREIGN KEY ([tag_id]) REFERENCES [tag] ([tag_id])
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'post_comment')
BEGIN
  CREATE TABLE [post_comment] (
    [post_comment_id] INT IDENTITY (1, 1) PRIMARY KEY CLUSTERED NOT NULL,
    [post_comment_guid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [post_id] INT NOT NULL,
    [user_id] INT NOT NULL,
    [content] NVARCHAR(1000) NOT NULL,
    [date_created] DATETIME NOT NULL DEFAULT GETUTCDATE()
  )

  ALTER TABLE [post_comment] ADD FOREIGN KEY ([post_id]) REFERENCES [post] ([post_id])

  ALTER TABLE [post_comment] ADD FOREIGN KEY ([user_id]) REFERENCES [user] ([user_id])
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE [name] = 'post_like')
BEGIN
  CREATE TABLE [post_like] (
    [post_like_id] INT IDENTITY (1, 1) PRIMARY KEY CLUSTERED NOT NULL,
    [post_id] INT NOT NULL,
    [user_id] INT NOT NULL,
    [date_created] DATETIME NOT NULL DEFAULT GETUTCDATE()
  )

  ALTER TABLE [post_like] ADD FOREIGN KEY ([post_id]) REFERENCES [post] ([post_id])

  ALTER TABLE [post_like] ADD FOREIGN KEY ([user_id]) REFERENCES [user] ([user_id])
END
GO
