using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _0bserv.Models
{
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
}
