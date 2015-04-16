CREATE TABLE [dbo].[VersionReferenceLink]
(
	[Id] int identity(1, 1) not null
		constraint PK_VersionReferenceLink_Id primary key clustered,
	VersionId int not null
		constraint FK_VersionReferenceLink_VersionId foreign key references dbo.Version(Id)
		on delete cascade,
	Href varchar(2000) not null
)
