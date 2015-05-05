CREATE TABLE [dbo].[Version]
(
	Id int identity(1, 1) not null
		constraint PK_Version_Id primary key clustered
        constraint CK_Version_Id_NotNegativeOne check (Id <> -1),
	Major int not null,
	Minor int not null,
	Build int not null,
	Revision int not null,
	FriendlyNameShort varchar(32) not null,
	FriendlyNameLong varchar(128) not null,
	IsSupported bit not null,
	ReleaseDate date null,
	constraint UK_Version_MajorMinorBuild unique (Major, Minor, Build)
)