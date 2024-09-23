CREATE PROCEDURE [dbo].[GetAccountSessions]
	@accountId int,
	@onlyActive bit = 0
AS
IF @onlyActive = 1
	SELECT * FROM [AccountSession] WHERE [AccountSession].[AccountId] = @accountId AND [AccountSession].[Active] = 1
ELSE
	SELECT * FROM [AccountSession] WHERE [AccountSession].[AccountId] = @accountId 
RETURN 0
