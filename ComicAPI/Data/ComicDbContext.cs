// generate data context 
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ComicApp.Data
{
    public class ComicDbContext : DbContext
    {
        public ComicDbContext(DbContextOptions<ComicDbContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Models.Comic> Comics => base.Set<Models.Comic>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<ComicGenre> ComicGenre => Set<ComicGenre>();
        public DbSet<Chapter> Chapters => Set<Chapter>();
        // public DbSet<Page> Pages => Set<Page>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("_USER").HasKey(x => x.ID);

            modelBuilder.Entity<Chapter>(b =>
            {
                b.ToTable("CHAPTER");
                b.HasKey(x => x.ID);
                b.HasMany(e => e.Pages)
                .WithOne()
                .HasForeignKey(x => x.ChapterID);
            });
            modelBuilder.Entity<Genre>().ToTable("GENRE").HasKey(x => x.ID);
            modelBuilder.Entity<Models.Comic>(b =>
            {
                b.ToTable("COMIC");
                b.HasKey(x => x.ID);
                b.HasMany(e => e.genres).WithMany().UsingEntity<ComicGenre>();
            });
            modelBuilder.Entity<ComicGenre>(b =>
            {
                b.ToTable("COMIC_GENRE");
                b.HasKey(x => new { x.ComicID, x.GenreID });
                b.HasOne(x => x.Comic).WithMany().HasForeignKey(x => x.ComicID);
                b.HasOne(x => x.Genre).WithMany().HasForeignKey(x => x.GenreID);
            }
            );


            // modelBuilder.Entity<Page>(b =>
            // {
            //     b.ToTable("PAGE");
            //     b.HasKey(x => x.ID);
            // });




            // .HasMany(e => e.genres)
            // .WithMany(e => e.comics)
            // .UsingEntity("Comic_Genre");
            // modelBuilder.Entity<Comics>();

        }

    }
}