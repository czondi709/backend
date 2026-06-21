using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KozossegAPI.Model
{
    [Table("Dokumentum")]
    public class Dokumentum
    {
        [Key]
        [Column("doku_id")]
        public int doku_id { get; set; }

        [Column("ceg_id")]
        public int ceg_id { get; set; }

        [ForeignKey(nameof(ceg_id))]
        public Ceg? Ceg { get; set; }

        [Column("pdf_link")]
        public required string pdf_link { get; set; }

        [Column("alairas_datuma")]
        public required DateTime alairas_datuma { get; set; }
    }
}
