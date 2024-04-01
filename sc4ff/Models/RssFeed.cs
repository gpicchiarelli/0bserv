using System;
using System.Collections.Generic;

namespace sc4ff.Models;

public partial class RssFeed
{
    public int Id { get; set; }

    public string Url { get; set; } = null!;

    public virtual ICollection<FeedContent> FeedContents { get; set; } = new List<FeedContent>();
}
