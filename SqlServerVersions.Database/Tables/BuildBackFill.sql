CREATE TABLE [dbo].[BuildBackFill]
(
    [Major] int not null,
    [Minor] int not null,
    [Build] int not null,
    [Revision] int not null
    constraint UK_VersionBackFill_MajorMinorBuild unique CLUSTERED (Major, Minor, Build)
)
