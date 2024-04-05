using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _0bserv.Models
{
    public class CredenzialiEmail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SmtpServer { get; set; }
        [Required]
        public int SmtpPort { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public byte[] EncryptedPassword { get; set; }         
    }
}
