/*******************************************************************************************
DATE          AUTHOR              NOTE
2025-05-18    Kyle Champion       Initial stored procedure to add post for logged in user.

Rules:
- Anonymous users not allowed to create post, user must be logged in.

Example usage:

EXEC AddPostProcedure
  @UserGuid = '34857ef8-8611-4c35-b188-034d6ae4d4b4',
  @Title = 'post title',
  @Content = 'post content'

*******************************************************************************************/
CREATE OR ALTER PROCEDURE AddPostProcedure
  @UserGuid UNIQUEIDENTIFIER,
  @Title NVARCHAR(200),
  @Content NVARCHAR(1000)
AS
BEGIN
  ---------------------------
  -- 1. Confirm user exists
  ---------------------------
  DECLARE @UserId INT;

  SELECT @UserId = [user_id]
  FROM [dbo].[user]
  WHERE [user_guid] = @UserGuid

  IF @UserId IS NULL
    THROW 50000, 'User not found', 1;

  ---------------------------
  -- 2. Insert post content
  ---------------------------
  INSERT INTO [dbo].[post]
  ([title], [content], [user_id])
  VALUES
  (@Title, @Content, @UserId)

  ---------------------------
  -- 3. Return with newly created post guid
  ---------------------------
  SELECT [post_guid]
  FROM [dbo].[post]
  WHERE [post_id] = SCOPE_IDENTITY()
END
