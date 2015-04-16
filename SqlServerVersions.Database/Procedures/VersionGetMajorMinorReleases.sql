CREATE PROCEDURE [dbo].[VersionGetMajorMinorReleases]
AS
	set nocount on;

	;with MajorMinorCte as
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
				row_number() over(partition by Major, Minor order by Major, Minor, Build, Revision)
		from dbo.Version
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
	from MajorMinorCte
	where row_num = 1
	order by Major, Minor, Build, Revision;
go
