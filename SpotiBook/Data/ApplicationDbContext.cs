using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpotiBook.Models;

namespace SpotiBook.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { 
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<FollowerRelation> FollowerRelations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FollowerRelation>()
                .HasKey(fr => new { fr.FollowerId, fr.FollowingId });

            builder.Entity<FollowerRelation>()
                .HasOne(fr => fr.Follower)
                .WithMany(fr => fr.Followers)
                .HasForeignKey(fr => fr.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FollowerRelation>()
                .HasOne(fr => fr.Following)
                .WithMany(fr => fr.Following)
                .HasForeignKey(fr => fr.FollowingId);
        }
    }
}
