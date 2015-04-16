CREATE TABLE [dbo].[ErrorLog]
(
    [Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY, 
    [LogDateTime] DATETIME NOT NULL DEFAULT getdate(), 
    [Type] NVARCHAR(MAX) NULL,
    [Message] NVARCHAR(MAX) NOT NULL, 
    [StackTrace] NVARCHAR(MAX) NULL
)
