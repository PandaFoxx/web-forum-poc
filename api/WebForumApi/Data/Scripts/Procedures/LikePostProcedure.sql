/*******************************************************************************************
DATE          AUTHOR              NOTE
2025-05-18    Kyle Champion       Initial stored procedure to like a post.

Rules:
- Anonymous users not allowed to like posts, user must be logged in.
- Each user can only like a post once.
- Users cannot like their own post.

Example usage:

EXEC LikePostProcedure
  @UserGuid = 'E0689550-A67B-4C3F-BA3F-6B08FA63D829',
  @PostGuid = 'FADD2CF9-A556-4A72-BAB9-161F89BE7892',
  @Unlike = 0

*******************************************************************************************/
CREATE OR ALTER PROCEDURE LikePostProcedure
  @UserGuid UNIQUEIDENTIFIER,
  @PostGuid UNIQUEIDENTIFIER,
  @Unlike BIT
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
  -- 3. Prevent user from liking their own post
  ---------------------------
  IF EXISTS
  (
    SELECT 1
    FROM [dbo].[post]
    WHERE @Unlike = 0
	  AND [post_id] = @PostId
    AND [user_id] = @UserId
  )
    THROW 50001, 'Unable to like own post', 1;

  ---------------------------
  -- 4. Prevent user from liking a post again
  ---------------------------
  IF EXISTS
  (
    SELECT 1
    FROM [dbo].[post_like]
    WHERE @Unlike = 0
	  AND [post_id] = @PostId
    AND [user_id] = @UserId
  )
    THROW 50001, 'Unable to like post again', 1;

  ---------------------------
  -- 5. Insert or delete like
  ---------------------------
  IF @Unlike = 0
  BEGIN
    INSERT INTO [dbo].[post_like]
    ([post_id], [user_id])
    VALUES
    (@PostId, @UserId);

	  SELECT SCOPE_IDENTITY() AS [post_like_id];
  END
  ELSE
  BEGIN
    DELETE FROM [dbo].[post_like]
    WHERE [post_id] = @PostId
    AND [user_id] = @UserId;

	  SELECT 0 AS [post_like_id];
  END
END