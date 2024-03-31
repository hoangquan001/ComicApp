// generate data context 
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ComicApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        public DbSet<Users> Users => Set<Users>();
        public DbSet<Comics> Comics => Set<Comics>();
        public DbSet<Genres> Genres => Set<Genres>();
        public DbSet<ComicGenre> ComicGenre => Set<ComicGenre>();
        public DbSet<Chapters> Chapters => Set<Chapters>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().ToTable("Users").HasKey(x => x.id);

            modelBuilder.Entity<Chapters>(b =>
            {
                b.ToTable("Chapters");
                b.HasKey(x => x.id);
                b.HasOne(x => x.comic);
            });
            modelBuilder.Entity<Genres>().ToTable("Genres").HasKey(x => x.id);
            modelBuilder.Entity<Comics>(b =>
            {
                b.ToTable("Comics");
                b.HasKey(x => x.id);
            });
            modelBuilder.Entity<ComicGenre>(b =>
            {
                b.ToTable("Comic_Genre");
                b.HasKey(x => new { x.comicid, x.genreid });
                // b.HasOne("comic").WithMany("Comics").HasForeignKey("comicid");
                b.HasOne(x => x.genre).WithMany().HasForeignKey(x => x.genreid);
            }
            );






            // .HasMany(e => e.genres)
            // .WithMany(e => e.comics)
            // .UsingEntity("Comic_Genre");
            // modelBuilder.Entity<Comics>();

        }

    }
}