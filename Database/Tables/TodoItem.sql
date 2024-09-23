CREATE TABLE [dbo].[TodoItem]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Title] NVARCHAR(MAX) NOT NULL, 
    [AccountId] INT NOT NULL,
    CONSTRAINT FK_TodoItemAccountId FOREIGN KEY ([AccountId]) REFERENCES [Account]
)
