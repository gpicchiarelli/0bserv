using System;

namespace _0bserv.Models
{
    public class Contenuto
    {
        public int Id { get; set; }
        public string Titolo { get; set; }
        public string Autore { get; set; }
        public DateTime? DataPubblicazione { get; set; }
        public string Testo { get; set; }
        public string Collegamento {get; set;}
    }
}