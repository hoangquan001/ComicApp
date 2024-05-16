// generate data context 
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;

namespace ComicApp.Data
{
    public class ComicDbContext : DbContext
    {
        public ComicDbContext(DbContextOptions<ComicDbContext> options)
            : base(options)
        {


        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Comic> Comics => base.Set<Comic>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<ComicGenre> ComicGenre => Set<ComicGenre>();

        public DbSet<UserFollowComic> UserFollowComics => Set<UserFollowComic>();
        public DbSet<Chapter> Chapters => Set<Chapter>();
        // public DbSet<Page> Pages => Set<Page>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("_user").HasKey(x => x.ID);

            modelBuilder.Entity<Chapter>(b =>
            {
                b.ToTable("chapter");
                b.HasKey(x => x.ID);
            });
            modelBuilder.Entity<Genre>().ToTable("genre").HasKey(x => x.ID);
            modelBuilder.Entity<Models.Comic>(b =>
            {
                b.ToTable("comic");
                b.HasKey(x => x.ID);
                b.HasMany(e => e.Genres).WithMany().UsingEntity<ComicGenre>();
                b.HasMany(e => e.Chapters).WithOne().HasForeignKey(x => x.ComicID);
            });
            modelBuilder.Entity<ComicGenre>(b =>
            {
                b.ToTable("comic_genre");
                b.HasKey(x => new { x.ComicID, x.GenreID });
                b.HasOne(x => x.Comic).WithMany().HasForeignKey(x => x.ComicID);
                b.HasOne(x => x.Genre).WithMany().HasForeignKey(x => x.GenreID);
            }
            );

            modelBuilder.Entity<UserFollowComic>(b =>
            {
                b.ToTable("user_follow_comic");
                b.HasKey(x => new { x.ComicID, x.UserID });
                b.HasOne(x => x.comic).WithMany().HasForeignKey(x => x.ComicID);
            });


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