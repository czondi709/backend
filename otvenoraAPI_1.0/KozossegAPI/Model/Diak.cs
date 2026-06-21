using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KozossegAPI.Model
{
    [Table("Diak")]
    public class Diak
    {
        [Key]
        [Column("diak_id")]
        public int diak_id { get; set; }

        [Column("nev")]
        public required string nev { get; set; }

        [Column("email")]
        public required string email { get; set; }

        [Column("jelszo")]
        public required string jelszo { get; set; }

        [Column("iskola")]
        public string? iskola { get; set; }

        [Column("telefonszam")]
        public string? telefonszam { get; set; }

        [Column("igazolas_pdf")]
        public string? igazolas_pdf { get; set; }

        [Column("ledolgozott_ora")]
        public int ledolgozott_ora { get; set; } = 0;

        [Column("om_azonosito")]
        public string? om_azonosito { get; set; } = null;

        [Column("iskola_om_kod")]
        public string? iskola_om_kod { get; set; } = null;

        [Column("osztaly")]
        public string? osztaly { get; set; } = null;

        [Column("is_active")]
        public int is_active { get; set; }
        [Column("verification_token")]
        public string? verification_token { get; set; }     
    }
}
