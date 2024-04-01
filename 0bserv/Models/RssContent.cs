using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _0bserv.Models
{
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
