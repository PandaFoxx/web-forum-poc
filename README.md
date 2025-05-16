# Web Forum POC
API and datastore backend of a web forum for a small number of users (&lt; 100). The forum is a basic text system which has the capabilities to add posts, retrieve posts, and like posts. Management does not believe in users editing or deleting existing posts, for ethical reasons

# Plan

## Summary:
  - posts created by users
  - posts have comments created by users
  - posts have likes added by users
  - one like per user
  - user cannot like own post
  - anonymous users can view posts
  - logged in user
    - regular user
      - create post
      - add comment
      - like post
      - unlike post
    - moderator user
      - ASSUME same as normal user
      - tag posts
  - get posts
    - with comments
    - with like count
    - filter date range
    - filter author
    - filter tag
    - sort by date
    - sort by like count

## Data Plan:

  https://dbdiagram.io/d/68272cb01227bdcb4ea2a66e

  - user
    - user_id
    - user_guid
    - user_category_id
    - first_name - encrypted
    - last_name - encrypted
    - email - encrypted
    - username - encrypted
    - password - encrypted
    - date_created
  - user_category
    - user_category_id
    - user_category_name (regular, moderator)
  - post
    - post_id
    - post_guid
    - title
    - content
    - user_id
    - date_created
  - post_comment
    - post_comment_id
    - post_comment_guid
    - post_id
    - user_id
    - content
    - date_created
  - post_like
    - post_like_id
    - post_id
    - user_id
    - date_created
  - post_tag
    - post_tag_id
    - tag_id
    - user_id
    - date_created
  - tag
    - tag_id
    - tag_name (misleading or false information)
