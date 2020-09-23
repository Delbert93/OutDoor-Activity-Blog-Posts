using System;
using System.Collections.Generic;
using System.Text;
using HW_6.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HW_6.Data
{
    public class ApplicationDbContext : IdentityDbContext<OutdoorUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> Pages { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BlogPost>()
                .HasMany(pt => pt.Comments);
           
           
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            base.OnModelCreating(builder);

            builder.Entity<OutdoorUser>(b =>
            {
                b.Property(u => u.UserName).HasMaxLength(128);
                b.Property(u => u.NormalizedUserName).HasMaxLength(128);
                b.Property(u => u.Email).HasMaxLength(128);
                b.Property(u => u.NormalizedEmail).HasMaxLength(128);
                b.Property(u => u.Address).HasMaxLength(128);
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.Property(t => t.LoginProvider).HasMaxLength(128);
                b.Property(t => t.Name).HasMaxLength(128);
            });
        }

        public DbSet<Comment> Comments { get; set; }
    }
}
