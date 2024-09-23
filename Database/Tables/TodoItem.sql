CREATE TABLE [dbo].[TodoItem]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Title] NVARCHAR(MAX) NOT NULL, 
    [AccountId] INT NOT NULL,
    CONSTRAINT FK_UserId FOREIGN KEY ([AccountId]) REFERENCES [Account]
)
