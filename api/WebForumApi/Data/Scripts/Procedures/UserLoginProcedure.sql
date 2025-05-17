/*******************************************************************************************
DATE          AUTHOR              NOTE
2025-05-17    Kyle Champion       Initial stored procedure to confirm user login credentials
                                  and return their identifier.

Example usage:

EXEC UserLoginProcedure
    @passphrase = 'secret_passphrase',
    @username = 'username',
    @password = 'password'

*******************************************************************************************/
CREATE OR ALTER PROCEDURE UserLoginProcedure
    @passphrase VARCHAR(64),
    @username NVARCHAR(100),
    @password NVARCHAR(100)
AS
BEGIN
  SELECT [user_guid] AS UserGuid
  FROM [dbo].[user]
  WHERE @username = CAST(DECRYPTBYPASSPHRASE(@passphrase, [username]) AS NVARCHAR(MAX))
  AND @password = CAST(DECRYPTBYPASSPHRASE(@passphrase, [password]) AS NVARCHAR(MAX))
END
