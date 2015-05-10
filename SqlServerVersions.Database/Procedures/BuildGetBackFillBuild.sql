CREATE PROCEDURE [dbo].[BuildGetBackFillBuild]
    @major int,
    @minor int,
    @build int
AS
    select
        Major,
        Minor,
        Build,
        Revision
    from dbo.BuildBackFill
    where Major = @major
    and Minor = @minor
    and Build = @build;
go
