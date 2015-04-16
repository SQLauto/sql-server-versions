CREATE PROCEDURE [dbo].[VersionAdd]
	@Major int,
	@Minor int,
	@Build int,
	@Revision int,
	@FriendlyNameShort varchar(32),
	@FriendlyNameLong varchar(128),
	@IsSupported bit,
	@ReleaseDate date = null
AS
	set nocount on;

	begin try
		insert into dbo.Version
		(
			Major,
			Minor,
			Build,
			Revision,
			FriendlyNameShort,
			FriendlyNameLong,
			IsSupported,
			ReleaseDate
		)
		values
		(
			@Major,
			@Minor,
			@Build,
			@Revision,
			@FriendlyNameShort,
			@FriendlyNameLong,
			@IsSupported,
			@ReleaseDate
		);

		return 0;
	end try
	begin catch
		return -1;
	end catch
go