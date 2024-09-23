CREATE TABLE [dbo].[AccountSession]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [AccountId] INT NOT NULL,
    [Active] BIT NOT NULL ,
    [Created] DATETIME NOT NULL DEFAULT GETDATE(),
    [LastAuthenticated] DATETIME NOT NULL,
    [Token] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
    CONSTRAINT FK_AccountSessionAccountId FOREIGN KEY ([AccountId]) REFERENCES [Account]
)
