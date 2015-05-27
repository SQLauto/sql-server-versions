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
go