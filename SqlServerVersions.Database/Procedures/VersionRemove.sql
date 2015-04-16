CREATE PROCEDURE [dbo].[VersionRemove]
	@Major int,
	@Minor int,
	@Build int,
	@Revision int
AS
	set nocount on;

	delete from dbo.Version
	where Major = @Major
	and Minor = @Minor
	and Build = @Build
	and Revision = @Revision;
go
