using CheeseHub.Models.Category;
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
        public DbSet<Category> Category { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<VideoReaction> VideoReactions{ get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        public DbSet<VideoView> VideoViews{ get; set; }
        public DbSet<Comment> Comments{ get; set; }

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
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(r => r.Videos)
                      .WithOne(x => x.User)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(r => r.Comments)
                  .WithOne(x => x.User)
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasIndex(x => x.Name).IsUnique();
                entity.HasMany(r => r.Users)
                      .WithOne(x => x.Role)
                      .HasForeignKey(x => x.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasIndex(x => x.Name).IsUnique();
                entity.HasMany(r => r.Videos)
                      .WithOne(x => x.Category)
                      .HasForeignKey(x => x.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasMany(r => r.Comments)
                      .WithOne(x => x.Video)
                      .HasForeignKey(x => x.VideoId)
                      .OnDelete(DeleteBehavior.Restrict); // Zmiana na Restrict
                entity.HasMany(r => r.Views)
                      .WithOne(x => x.Video)
                      .HasForeignKey(x => x.VideoId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(r => r.Reactions)
                      .WithOne(x => x.Video)
                      .HasForeignKey(x => x.TargetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.User) 
                      .WithMany(u => u.Comments) 
                      .HasForeignKey(c => c.UserId) 
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(c => c.Video)
                      .WithMany(v => v.Comments)
                      .HasForeignKey(c => c.VideoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ParentComment)
                      .WithMany(c => c.Replies)
                      .HasForeignKey(c => c.ParentCommentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VideoReaction>(entity =>
            {
                entity.HasOne(vr => vr.Video)
                      .WithMany(v => v.Reactions)
                      .HasForeignKey(vr => vr.TargetId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(vr => vr.User)
                      .WithMany()
                      .HasForeignKey(vr => vr.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CommentReaction>(entity =>
            {
                entity.HasOne(cr => cr.Comment)
                      .WithMany(c => c.Reactions)
                      .HasForeignKey(cr => cr.TargetId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(cr => cr.User)
                      .WithMany()
                      .HasForeignKey(cr => cr.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VideoView>(entity =>
            {
                entity.HasOne(vv => vv.Video)
                      .WithMany(v => v.Views)
                      .HasForeignKey(vv => vv.VideoId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(vv => vv.User)
                      .WithMany(u => u.VideoViews)
                      .HasForeignKey(vv => vv.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
