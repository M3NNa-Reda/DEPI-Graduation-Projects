using alealamia.Domain.Entities;
using alealamia.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ClientProfile> ClientProfiles { get; set; }
        public DbSet<FreelancerProfile> FreelancerProfiles { get; set; }
        public DbSet<FreelancerBlock> FreelancerBlocks { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<TrackSkill> TrackSkills { get; set; }
        public DbSet<FreelancerSkill> FreelancerSkills { get; set; }
        public DbSet<PortfolioItem> PortfolioItems { get; set; }
        public DbSet<PortfolioItemFile> p { get; set; }
        public DbSet<TaskSubmission> TaskSubmissions { get; set; }
        public DbSet<TaskReview> TaskReviews { get; set; }
        public DbSet<TaskRevision> TaskRevisions { get; set; }
        public DbSet<SubmissionFile> SubmissionFiles { get; set; }
        public DbSet<FreelancerLedgerEntry> FreelancerLedgerEntries { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            base.OnModelCreating(builder);
        }
    }
}
