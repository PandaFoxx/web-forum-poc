/*******************************************************************************************
DATE          AUTHOR              NOTE
2025-05-18    Kyle Champion       Initial stored procedure to tag a post.

Rules:
- Anonymous users not allowed to tag posts, user must be logged in.
- Only moderators can tag posts.

Example usage:

EXEC TagPostProcedure
  @UserGuid = 'E0689550-A67B-4C3F-BA3F-6B08FA63D829',
  @PostGuid = 'FADD2CF9-A556-4A72-BAB9-161F89BE7892',
  @TagId = 1

*******************************************************************************************/
CREATE OR ALTER PROCEDURE TagPostProcedure
  @UserGuid UNIQUEIDENTIFIER,
  @PostGuid UNIQUEIDENTIFIER,
  @TagId INT
AS
BEGIN
  ---------------------------
  -- 1. Confirm user exists and is a moderator
  ---------------------------
  DECLARE @UserId INT;

  SELECT @UserId = [user_id]
  FROM [dbo].[user]
  WHERE [user_guid] = @UserGuid
  AND [user_category_id] = 2 -- Moderator

  IF @UserId IS NULL
    THROW 50000, 'User not found or not a Moderator', 1;

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
  -- 3. Confirm tag exists
  ---------------------------
  IF NOT EXISTS
  (
    SELECT 1
    FROM [dbo].[tag]
    WHERE [tag_id] = @TagId
  )
    THROW 50000, 'Tag not found', 1;

  ---------------------------
  -- 4. Tag post
  ---------------------------
  UPDATE [dbo].[post]
  SET [tag_id] = @TagId
  WHERE [post_id] = @PostId

END