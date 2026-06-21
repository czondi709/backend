using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KozossegAPI.Model
{
    [Table("Munka")]
    public class Munka
    {
        [Key]
        [Column("munka_id")]
        public int munka_id { get; set; }
       
        [Column("ceg_id")]
        public int ceg_id { get; set; }

        [ForeignKey(nameof(ceg_id))]
        public Ceg? Ceg { get; set; }   

        [Column("munka_nev")]
        public required string munka_nev { get; set; }

        [Column("cim")]
        public required string cim { get; set; }
       
        [Column("idopont")]
        public required DateTime idopont { get; set; }
      
        [Column("oraszam")]
        public int oraszam { get; set; }

        [Column("letszam")]
        public int letszam { get; set; }
      
        [Column("leiras")]
        public required string leiras { get; set; }
       
        [Column("statusz")]
        public required string statusz { get; set; } = "Aktív";

        [Column("kategoria")]
        public required string kategoria { get; set; } = string.Empty;
    }
}
