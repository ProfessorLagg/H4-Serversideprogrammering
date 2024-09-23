CREATE TABLE [dbo].[Account]
(
	[Id] INT NOT NULL PRIMARY KEY,
    [login] NVARCHAR(MAX) NOT NULL, 
    [CPR] NVARCHAR(MAX) NULL, 
    [PasswordHash] CHAR(192) NOT NULL,

)
