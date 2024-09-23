CREATE PROCEDURE [dbo].[GetAccountSessions]
	@userId int,
	@onlyActive bit = 0
AS
IF @onlyActive = 1
	SELECT * FROM [AccountSession] WHERE [AccountSession].[UserId] = @userId AND [AccountSession].[Active] = 1
ELSE
	SELECT * FROM [AccountSession] WHERE [AccountSession].[UserId] = @userId 
RETURN 0
