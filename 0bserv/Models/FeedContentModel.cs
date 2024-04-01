using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _0bserv.Models
{
    public class FeedContentModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RssFeedId { get; set; }

        [ForeignKey("RssFeedId")]
        public virtual required FeedModel RssFeed { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required string Link { get; set; }

        [Required]
        public required string Author { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }
    }
}
