using Microsoft.EntityFrameworkCore;
using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Data
{
    public class HelpdeskDbContext : DbContext
    {
        public HelpdeskDbContext(DbContextOptions<HelpdeskDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Users_Email");
                
                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ModifiedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.ModifiedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2");
                entity.Property(e => e.LastLogin).HasColumnType("datetime2");
                entity.Property(e => e.LastPasswordChange).HasColumnType("datetime2");
            });

            // Configure Ticket entity
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.AssignedToUser)
                      .WithMany()
                      .HasForeignKey(e => e.AssignedToUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ModifiedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.ModifiedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2");
                entity.Property(e => e.ResolvedDate).HasColumnType("datetime2");
                entity.Property(e => e.ClosedDate).HasColumnType("datetime2");

                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.CreatedDate);
            });

            modelBuilder.Entity<TicketAttachment>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Ticket)
                      .WithMany(t => t.Attachments)
                      .HasForeignKey(e => e.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.UploadedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.UploadedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.UploadedDate).HasColumnType("datetime2");
            });

            // Configure TicketComment entity
            modelBuilder.Entity<TicketComment>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Ticket)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(e => e.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ModifiedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.ModifiedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2");
            });
        }
    }
}