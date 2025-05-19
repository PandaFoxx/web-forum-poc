USE [iidentifii-web-forum];

SET IDENTITY_INSERT [dbo].[user_category] ON

MERGE [dbo].[user_category] tar
USING
  (
    VALUES
       (1, 'Regular')
      ,(2, 'Moderator')
  )
  src ([user_category_id], [user_category_name])
ON tar.[user_category_id] = src.[user_category_id]
WHEN MATCHED THEN
  UPDATE SET
    [user_category_name] = src.[user_category_name]
WHEN NOT MATCHED BY TARGET THEN
  INSERT
    ([user_category_id], [user_category_name])
  VALUES
    (src.[user_category_id], src.[user_category_name]);

SET IDENTITY_INSERT [dbo].[user_category] OFF

----------------------------------------------------------------

SET IDENTITY_INSERT [dbo].[tag] ON

MERGE [dbo].[tag] tar
USING
  (
    VALUES
      (1, 'Misleading or false information')
  )
  src ([tag_id], [tag_name])
ON tar.[tag_id] = src.[tag_id]
WHEN MATCHED THEN
  UPDATE SET
    [tag_name] = src.[tag_name]
WHEN NOT MATCHED BY TARGET THEN
  INSERT
    ([tag_id], [tag_name])
  VALUES
    (src.[tag_id], src.[tag_name]);

SET IDENTITY_INSERT [dbo].[tag] OFF
