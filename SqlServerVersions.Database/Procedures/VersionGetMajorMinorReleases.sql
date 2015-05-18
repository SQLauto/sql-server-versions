CREATE PROCEDURE [dbo].[VersionGetMajorMinorReleases]
AS
	set nocount on;

	select
        Id,
		Major,
		Minor,
		Build,
		Revision,
		FriendlyNameShort,
		FriendlyNameLong,
		IsSupported,
		ReleaseDate
	from dbo.Version
	where IsMajorRelease = 1
	order by Major, Minor, Build, Revision;
go
