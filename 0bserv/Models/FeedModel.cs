using System.ComponentModel.DataAnnotations;

namespace _0bserv.Models
{
    public class FeedModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public required string Url { get; set; }

        // Altre proprietà se necessario

        // Relazione con il contenuto dei feed
        public virtual ICollection<FeedContentModel> Contents { get; set; }
    }
}
