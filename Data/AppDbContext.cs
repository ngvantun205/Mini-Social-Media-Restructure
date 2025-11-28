using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mini_Social_Media.Models.DomainModel;

namespace Mini_Social_Media.Data {
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int> {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostMedia> PostMedias { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<PostHashtag> PostHashtags { get; set; }
        public DbSet<Notifications> Notifications { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostMedia>()
                .HasOne(pm => pm.Post)
                .WithMany(p => p.Medias)
                .HasForeignKey(pm => pm.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasKey(l => new { l.UserId, l.PostId });

            builder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Follow>()
                .HasKey(f => new { f.FollowerId, f.FolloweeId });

            builder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Follow>()
                .HasOne(f => f.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostHashtag>()
                .HasKey(ph => new { ph.PostId, ph.HashtagId });

            builder.Entity<PostHashtag>()
                .HasOne(ph => ph.Post)
                .WithMany(p => p.PostHashtags)
                .HasForeignKey(ph => ph.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostHashtag>()
                .HasOne(ph => ph.Hashtag)
                .WithMany(h => h.PostHashtags)
                .HasForeignKey(ph => ph.HashtagId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notifications>()
                .HasOne(n => n.Receiver)
                .WithMany(u => u.ReceivedNotifications)
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notifications>()
                .HasOne(n => n.Actor)
                .WithMany(u => u.SentNotifications)
                .HasForeignKey(n => n.ActorId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }

}
