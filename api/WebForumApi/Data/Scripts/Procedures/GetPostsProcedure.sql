/*******************************************************************************************
DATE          AUTHOR              NOTE
2025-05-18    Kyle Champion       Initial stored procedure to get posts for anonymous users.

Rules:
- No restriction on user category.
- Include comments
- Include likes
- Include tags
- Include users (authors of posts)
- Include users (authors of comments)
- Filter by date range of posts
- Filter by user of posts
- Filter by tag
- Sort by date
- Sort by like count
- Paging

Example usage:

EXEC GetPostsProcedure
	@Passphrase = 'secret',
	--@FilterUserGuid = NULL,
	--@FilterDateFrom = NULL,
	--@FilterDateTo = NULL,
	--@FilterTagId = NULL,
	--@SortColumn = NULL,
	--@SortDirection = NULL,
	@PageSize = 4,
	@PageNumber = 1

*******************************************************************************************/
CREATE OR ALTER PROCEDURE GetPostsProcedure
    @Passphrase VARCHAR(64),
	--@FilterUserGuid UNIQUEIDENTIFIER,
	--@FilterDateFrom DATETIME,
	--@FilterDateTo DATETIME,
	--@FilterTagId INT,
	--@SortColumn VARCHAR(50),
	--@SortDirection VARCHAR(4),
	@PageSize INT,
	@PageNumber INT
AS
BEGIN
	---------------------------
	-- 1. Count likes per post
	---------------------------
	DECLARE @PostLikes TABLE
	(
		 [post_id] INT
		,[like_count] INT
	);
	INSERT INTO @PostLikes
	SELECT
		 p.[post_id]
		,COUNT(l.[user_id]) AS [like_count]
	FROM [dbo].[post] p
	LEFT JOIN [dbo].[post_like] l ON l.[post_id] = p.[post_id]
	GROUP BY p.[post_id]

	---------------------------
	-- 2. Get posts, comments, likes, tags and users
	---------------------------
	SELECT
		 p.[post_guid]
		,p.[title] AS [post_title]
		,p.[content] AS [post_content]
		,p.[date_created] AS [post_date_created]
		,pu.[user_guid] AS [post_user_guid]
		,CONCAT
		(
			 CAST(DECRYPTBYPASSPHRASE(@Passphrase, pu.[first_name]) AS NVARCHAR(MAX))
			,' '
			,CAST(DECRYPTBYPASSPHRASE(@Passphrase, pu.[last_name]) AS NVARCHAR(MAX))
		) AS [post_user_name]
		,pl.[like_count]
		,t.[tag_id]
		,t.[tag_name]
		,c.[content] AS [comment_content]
		,c.[date_created] AS [comment_date_created]
		,CONCAT
		(
			 CAST(DECRYPTBYPASSPHRASE(@Passphrase, cu.[first_name]) AS NVARCHAR(MAX))
			,' '
			,CAST(DECRYPTBYPASSPHRASE(@Passphrase, cu.[last_name]) AS NVARCHAR(MAX))
		) AS [comment_user_name]
	FROM [dbo].[post] p
	LEFT JOIN [dbo].[post_comment] c ON c.[post_id] = p.[post_id]
	INNER JOIN @PostLikes pl ON pl.[post_id] = p.[post_id]
	INNER JOIN [dbo].[user] pu ON pu.[user_id] = p.[user_id]
	INNER JOIN [dbo].[user] cu ON cu.[user_id] = c.[user_id]
	LEFT JOIN [dbo].[tag] t ON t.[tag_id] = p.[tag_id]
	ORDER BY p.[date_created]
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY;
END
