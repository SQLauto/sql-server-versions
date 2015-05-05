CREATE PROCEDURE [dbo].[BuildRemoveBackFill]
    @Major int,
    @Minor int,
    @Build int
AS
    set nocount on;

    delete
    from dbo.BuildBackFill
    where Major = @Major
    and Minor = @Minor
    and Build = @Build;
go
