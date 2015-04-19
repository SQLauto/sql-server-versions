CREATE PROCEDURE [dbo].[BuildGetRandomBackFill]
AS
    set nocount on;

    select top 1
        Major,
        Minor,
        Build,
        Revision,
        RandomId = NEWID()
    from dbo.BuildBackFill
    order by RandomId;
go