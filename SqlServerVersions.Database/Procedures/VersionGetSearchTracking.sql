CREATE PROCEDURE [dbo].[VersionGetSearchTracking]
AS
    select
        v.Major,
        v.Minor,
        v.Build,
        v.Revision,
        version_count = count(*)
    from dbo.VersionSearchTracking vst
    inner join dbo.Version v
    on vst.VersionId = v.Id
    group by v.Major, v.Minor, v.Build, v.Revision
    order by version_count desc;
go