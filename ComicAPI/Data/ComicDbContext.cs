// generate data context 
using ComicAPI.Models;
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
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<UserNotification> Notifications => Set<UserNotification>();
        public DbSet<UserVoteComic> UserVoteComics => Set<UserVoteComic>();
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
                // b.HasQueryFilter(x => !(x.UpdateAt < new DateTime(2018, 1, 1).ToUniversalTime() && x.Chapters.Count < 15));
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

            modelBuilder.Entity<UserVoteComic>(b =>
            {
                b.ToTable("user_vote_comic");
                b.HasKey(x => new { x.ComicID, x.UserID });

            });

            modelBuilder.Entity<Comment>(b =>
            {
                b.ToTable("comment");
                b.HasKey(x => new { x.ID });
                b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserID);
                b.HasMany(x => x.Replies).WithOne().HasForeignKey(x => x.ParentCommentID);
            });

            modelBuilder.Entity<UserNotification>(b =>
            {
                b.ToTable("user_notification");
                b.HasKey(x => new { x.ID });
            });

            modelBuilder.HasDbFunction(() => GetLatestChapter(default))
            .HasName("get_latest_chapter");

        }
        [DbFunction("public", "get_latest_chapter")]
        public virtual IQueryable<Chapter> GetLatestChapter(int comicId)
        {
            var parameter = new Npgsql.NpgsqlParameter("comic_id", comicId);
            return this.Set<Chapter>().FromSqlRaw("SELECT * FROM get_latest_chapter(@comic_id)", parameter);
        }
    }
}