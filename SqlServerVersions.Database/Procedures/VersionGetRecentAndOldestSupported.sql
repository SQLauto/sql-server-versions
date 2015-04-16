CREATE PROCEDURE [dbo].[VersionGetRecentAndOldestSupported]
	@Major int = null,
	@Minor int = null
AS
	set nocount on;

	;with VersionOld as
	(
		select
            Id,
			Major,
			Minor,
			Build,
			Revision,
			FriendlyNameShort,
			FriendlyNameLong,
			IsSupported,
			ReleaseDate,
			row_num = 
				row_number() over (partition by Major, Minor order by Major, Minor, Build, Revision)
		from dbo.Version
		where
		(
			@Major is null
			or Major = @Major
		) and
		(
			@Minor is null
			or Minor = @Minor
		)
		and IsSupported = 1
	), VersionNew as
	(
		select
            Id,
			Major,
			Minor,
			Build,
			Revision,
			FriendlyNameShort,
			FriendlyNameLong,
			IsSupported,
			ReleaseDate,
			row_num = 
				row_number() over (partition by Major, Minor order by Major desc, Minor desc, Build desc, Revision desc)
		from dbo.Version
		where
		(
			@Major is null
			or Major = @Major
		) and
		(
			@Minor is null
			or Minor = @Minor
		)
		and IsSupported = 1
	)
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
	from VersionOld
	where row_num = 1
	union all
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
	from VersionNew
	where row_num = 1;
go