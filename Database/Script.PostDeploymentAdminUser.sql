/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


IF NOT EXISTS (SELECT Id FROM [Account] WHERE [Login] = 'admin')
BEGIN
    INSERT INTO [Account] ([Login],[PasswordHash])
    VALUES ('admin','9ca694a90285c034432c9550421b7b9dbd5c0f4b6673f05f6dbce58052ba20e4248041956ee8c9a2ec9f10290cdc0782')
END

GO

DECLARE @accId INT = (SELECT Id FROM [Account] WHERE [Login] = 'admin');
IF (SELECT COUNT([Id]) FROM [TodoItem] WHERE [AccountId] = 'admin') = 0
BEGIN
    INSERT INTO [TodoItem] ([Title],[AccountId])
    VALUES
        ('Remember to do the stuff',@accId),
        ('Did you do the thing?',@accId),
        ('Send some emails',@accId)
END