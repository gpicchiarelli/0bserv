IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [RssFeeds] (
    [Id] int NOT NULL IDENTITY,
    [Url] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_RssFeeds] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FeedContents] (
    [Id] int NOT NULL IDENTITY,
    [RssFeedId] int NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Link] nvarchar(max) NOT NULL,
    [Author] nvarchar(max) NOT NULL,
    [PublishDate] datetime2 NOT NULL,
    CONSTRAINT [PK_FeedContents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FeedContents_RssFeeds_RssFeedId] FOREIGN KEY ([RssFeedId]) REFERENCES [RssFeeds] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_FeedContents_PublishDate] ON [FeedContents] ([PublishDate]);
GO

CREATE INDEX [IX_FeedContents_RssFeedId] ON [FeedContents] ([RssFeedId]);
GO

CREATE UNIQUE INDEX [IX_RssFeeds_Url] ON [RssFeeds] ([Url]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240330203957_AggiuntaIndiceUnivocoSuUrl', N'8.0.3');
GO

COMMIT;
GO


