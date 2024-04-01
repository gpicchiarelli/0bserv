using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace sc4ff.Models;

public partial class _0bservContext : DbContext
{
    public _0bservContext()
    {
    }

    public _0bservContext(DbContextOptions<_0bservContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FeedContent> FeedContents { get; set; }

    public virtual DbSet<RssFeed> RssFeeds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=0bserv;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedContent>(entity =>
        {
            entity.HasIndex(e => e.PublishDate, "IX_FeedContents_PublishDate");

            entity.HasIndex(e => e.RssFeedId, "IX_FeedContents_RssFeedId");

            entity.HasOne(d => d.RssFeed).WithMany(p => p.FeedContents).HasForeignKey(d => d.RssFeedId);
        });

        modelBuilder.Entity<RssFeed>(entity =>
        {
            entity.HasIndex(e => e.Url, "IX_RssFeeds_Url").IsUnique();

            entity.Property(e => e.Url).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
