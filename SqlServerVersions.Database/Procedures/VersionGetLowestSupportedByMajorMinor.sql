CREATE PROCEDURE [dbo].[VersionGetLowestSupportedByMajorMinor]
	@Major int,
	@Minor int
AS
	select top 1
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
	where Major = @Major
	and Minor = @Minor
	and IsSupported = 1
	order by Major, Minor, Build, Revision;
go