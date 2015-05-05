CREATE PROCEDURE [dbo].[VersionGet]
	@Major int = null,
	@Minor int = null,
	@Build int = null
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
	where
	(
		@Major is null
		or Major = @Major
	) and
	(
		@Minor is null
		or Minor = @Minor
	) and
	(
		@Build is null 
		or Build = @Build
	);
go
