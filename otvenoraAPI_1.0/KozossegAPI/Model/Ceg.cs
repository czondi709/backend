using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KozossegAPI.Model
{
    [Table("Ceg")]
    public class Ceg
    {
        [Key]
        [Column("ceg_id")]
        public int ceg_id { get; set; }

        [Column("cegnev")]
        public required string cegnev { get; set; }

        [Column("ceg_email")]
        public required string ceg_email { get; set; }
                                
        [Column("cim")]
        public required string cim { get; set; }

        [Column("adoszam")]
        public required string adoszam { get; set; }

        [Column("jelszo")]
        public required string jelszo { get; set; }

        [Column("telefonszam")]

        public required string telefonszam { get; set; }

        [Column("is_active")]
        public int is_active { get; set; }
        [Column("verification_token")]
        public string? verification_token { get; set; }



        public ICollection<Dokumentum> Dokumentumok { get; set; } = new List<Dokumentum>();
    }
}
