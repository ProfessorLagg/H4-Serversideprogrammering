CREATE TABLE [dbo].[Sessions]
(
	[Id] INT NOT NULL PRIMARY KEY,
    [UserId] INT NOT NULL,
    [Active] BIT NOT NULL ,
    [Created] DATETIME NOT NULL, 
    [LastUsed] DATETIME NULL    
)
