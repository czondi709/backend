using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KozossegAPI.Model
{
    [Table("Jelentkezes")]
    public class Jelentkezes
    {      
        [Column("munka_id")]
        public int munka_id { get; set; }

        [ForeignKey(nameof(munka_id))]
        public Munka? Munka { get; set; }

        [Column("diak_id")]
        public int diak_id { get; set; }

        [ForeignKey(nameof(diak_id))]
        public Diak? Diak { get; set; }

        [Column("jelentkezes_ideje")]
        public DateTime jelentkezes_ideje { get; set; } = DateTime.Now;

        [Column("munka_statusz")]
        public required string munka_statusz { get; set; } = "Elküldve";

        [Column("jovahagyott_ora")]
        public int jovahagyott_ora { get; set; } = 0;

        [Column("igazolas_adatok")]
        public string? igazolas_adatok { get; set; } = null;
    }
}
