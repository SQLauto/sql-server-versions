CREATE PROCEDURE [dbo].[VersionGetTopRecentRelease]
	@TopCount int,
    @Major int = null,
    @Minor int = null
AS
	set nocount on;

	select top (@TopCount)
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
    where (@Major is null or Major = @Major) 
    and (@Minor is null or Minor = @Minor)
	order by ReleaseDate desc;
go
