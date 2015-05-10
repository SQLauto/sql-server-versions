CREATE TABLE [dbo].[VersionSearchTracking]
(
    [VersionId] INT NOT NULL
        constraint FK_VersionSearchTracking_VersionId foreign key references [dbo].[Version] (Id)
        on delete cascade
)
GO

CREATE CLUSTERED INDEX [IX_VersionSearchTracking_VersionId] ON [dbo].[VersionSearchTracking] ([VersionId])
