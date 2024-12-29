using CheeseHub.Models.Comment;
using CheeseHub.Models.CommentReaction;
using CheeseHub.Models.RefreshToken;
using CheeseHub.Models.Role;
using CheeseHub.Models.User;
using CheeseHub.Models.Video;
using CheeseHub.Models.VideoReaction;
using CheeseHub.Models.VideoView;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace CheeseHub.Data
{
    public class ApplicationDbContext :DbContext
    {
        public DbSet<Video> Videos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(x => x.Email).IsUnique();   
                entity.HasIndex(x => x.Name).IsUnique();
                entity.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(r => r.Videos).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);


            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasIndex(x => x.Name).IsUnique();
                entity.HasMany(r => r.Users).WithOne(x => x.Role).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);
            modelBuilder.Entity<Comment>()
            .HasOne(c => c.Video)
            .WithMany(v => v.Comments)
            .HasForeignKey(c => c.VideoId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId);

            modelBuilder.Entity<VideoReaction>()
                .HasOne(vr => vr.Video)
                .WithMany(v => v.Reactions)
                .HasForeignKey(vr => vr.TargetId);

            modelBuilder.Entity<VideoReaction>()
                .HasOne(vr => vr.User)
                .WithMany()
                .HasForeignKey(vr => vr.UserId);

            modelBuilder.Entity<CommentReaction>()
                .HasOne(cr => cr.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(cr => cr.TargetId);

            modelBuilder.Entity<CommentReaction>()
                .HasOne(cr => cr.User)
                .WithMany()
                .HasForeignKey(cr => cr.UserId);

            modelBuilder.Entity<VideoView>()
                .HasOne(vv => vv.Video)
                .WithMany(v => v.Views)
                .HasForeignKey(vv => vv.TargetId);

            modelBuilder.Entity<VideoView>()
                .HasOne(vv => vv.User)
                .WithMany(u => u.VideoViews)
                .HasForeignKey(vv => vv.UserId);
        }
    }
}
