namespace _0bserv.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    public class RssFeed
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Url { get; set; }

        // Altre proprietà se necessario

        // Relazione con il contenuto dei feed
        public virtual ICollection<FeedContent> Contents { get; set; }
    }

    public class FeedContent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RssFeedId { get; set; }

        [ForeignKey("RssFeedId")]
        public virtual RssFeed RssFeed { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

    }
}
