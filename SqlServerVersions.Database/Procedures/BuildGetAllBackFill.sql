CREATE PROCEDURE [dbo].[BuildGetAllBackFill]
AS
    set nocount on;

    select
        Major,
        Minor,
        Build,
        Revision
    from dbo.BuildBackFill
    order by Major, Minor, Build, Revision;
go
