CREATE PROCEDURE [dbo].[VersionModify]
	@MajorOld int,
	@MinorOld int,
	@BuildOld int,
	@RevisionOld int,
	@Major int,
	@Minor int,
	@Build int,
	@Revision int,
	@FriendlyNameShort varchar(32),
	@FriendlyNameLong varchar(128),
	@IsSupported bit,
	@ReleaseDate date = null
AS
	begin try
		update dbo.Version
		set 
			Major = @Major,
			Minor = @Minor,
			Build = @Build,
			Revision = @Revision,
			FriendlyNameShort = @FriendlyNameShort,
			FriendlyNameLong = @FriendlyNameLong,
			IsSupported = @IsSupported,
			ReleaseDate = @ReleaseDate
		where Major = @MajorOld
		and Minor = @MinorOld
		and Build = @BuildOld
		and Revision = @RevisionOld;

		return 0;
	end try
	begin catch
		return -1;
	end catch
go
