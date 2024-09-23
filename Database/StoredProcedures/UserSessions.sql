CREATE PROCEDURE [dbo].[UserSessions]
	@userId int,
	@onlyActive bit = 0
AS
IF @onlyActive = 1
	SELECT * FROM [Sessions] WHERE [Sessions].[UserId] = @userId AND [Sessions].[Active] = 1
ELSE
	SELECT * FROM [Sessions] WHERE [Sessions].[UserId] = @userId 
RETURN 0
