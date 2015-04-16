CREATE PROCEDURE [dbo].[VersionGetMostRecentByMajorMinor]
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
	order by ReleaseDate desc;
go