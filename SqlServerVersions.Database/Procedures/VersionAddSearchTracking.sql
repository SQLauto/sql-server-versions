CREATE PROCEDURE [dbo].[VersionAddSearchTracking]
    @Major int,
    @Minor int,
    @Build int
AS
    insert into dbo.VersionSearchTracking (VersionId)
    select Id
    from dbo.Version
    where Major = @major
    and Minor = @Minor
    and Build = @Build;
go