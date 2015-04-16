CREATE PROCEDURE [dbo].[ErrorLogAddMessage]
    @Message nvarchar(max),
    @Type nvarchar(max) = null,
    @StackTrace nvarchar(max) = null
AS
    set nocount on;

    insert into dbo.ErrorLog 
    (
        LogDateTime,
        Type,
        Message,
        StackTrace
    )
    values (getdate(), @Type, @Message, @StackTrace);
go
