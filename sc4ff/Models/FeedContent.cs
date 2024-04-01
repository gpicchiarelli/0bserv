using System;
using System.Collections.Generic;

namespace sc4ff.Models;

public partial class FeedContent
{
    public int Id { get; set; }

    public int RssFeedId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Link { get; set; } = null!;

    public string Author { get; set; } = null!;

    public DateTime PublishDate { get; set; }

    public virtual RssFeed RssFeed { get; set; } = null!;
}
