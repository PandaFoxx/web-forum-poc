/*******************************************************************************************
DATE          AUTHOR              NOTE
2025-05-18    Kyle Champion       Initial stored procedure to add comment to post.

Rules:
- Anonymous users not allowed to comment on posts, user must be logged in.

Example usage:

EXEC AddPostCommentProcedure
  @UserGuid = 'E0689550-A67B-4C3F-BA3F-6B08FA63D829',
  @PostGuid = '812E4184-6673-4DCF-9516-55EA7637CA75',
  @Content = 'comment content'

*******************************************************************************************/
CREATE OR ALTER PROCEDURE AddPostCommentProcedure
  @UserGuid UNIQUEIDENTIFIER,
  @PostGuid UNIQUEIDENTIFIER,
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
  -- 2. Confirm post exists
  ---------------------------
  DECLARE @PostId INT;

  SELECT @PostId = [post_id]
  FROM [dbo].[post]
  WHERE [post_guid] = @PostGuid

  IF @PostId IS NULL
    THROW 50000, 'Post not found', 1;

  ---------------------------
  -- 3. Insert comment content
  ---------------------------
  INSERT INTO [dbo].[post_comment]
  ([post_id], [user_id], [content])
  VALUES
  (@PostId, @UserId, @Content)

  ---------------------------
  -- 4. Return with newly created comment guid
  ---------------------------
  SELECT [post_comment_guid]
  FROM [dbo].[post_comment]
  WHERE [post_comment_id] = SCOPE_IDENTITY()
END
