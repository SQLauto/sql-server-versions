CREATE PROCEDURE [dbo].[VersionAddReferenceLink]
	@Major int,
	@Minor int,
	@Build int,
	@NewReferenceLink varchar(2000)
AS
	set nocount on;

	insert into dbo.VersionReferenceLink
	(
		VersionId,
		Href
	)
	select
		Id,
		@NewReferenceLink
	from dbo.Version
	where Major = @Major
	and Minor = @Minor
	and Build = @Build;
go