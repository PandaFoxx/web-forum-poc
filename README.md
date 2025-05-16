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