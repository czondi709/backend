using Microsoft.EntityFrameworkCore;
using KozossegAPI.Model;

namespace KozossegAPI.Model
{
    public class KozossegDbContext : DbContext
    {
        public KozossegDbContext(DbContextOptions<KozossegDbContext> options) : base(options) { }

        public DbSet<Ceg> Cegek { get; set; }
        public DbSet<Munka> Munkak { get; set; }
        public DbSet<Diak> Diakok { get; set; }
        public DbSet<Dokumentum> Dokumentumok { get; set; }
        public DbSet<Jelentkezes> Jelentkezesek { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Jelentkezes>()
                .HasKey(j => new { j.munka_id, j.diak_id });

            modelBuilder.Entity<Munka>()
                .HasOne(m => m.Ceg)
                .WithMany()
                .HasForeignKey(m => m.ceg_id);
        }
    }
}