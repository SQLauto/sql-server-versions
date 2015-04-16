CREATE PROCEDURE [dbo].[VersionGetReferenceLinks]
	@Major int,
	@Minor int,
	@Build int,
	@Revision int
AS
	set nocount on;

	select
		vrl.Href
	from dbo.VersionReferenceLink vrl
	inner join dbo.Version v
	on vrl.VersionId = v.Id
	where v.Major = @Major
	and v.Minor = @Minor
	and v.Build = @Build
	and v.Revision = @Revision;
go