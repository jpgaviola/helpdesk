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
    public DbSet<DocumentRequest> DocumentRequests { get; set; }
    public DbSet<DocumentItem> DocumentItems { get; set; }
    public DbSet<DocumentRequestAttachment> DocumentRequestAttachments { get; set; }
    public DbSet<TicketSignatory> TicketSignatories { get; set; }

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

        entity.HasIndex(e => e.Role);
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => new { e.Role, e.Status });
        entity.HasIndex(e => e.Domain);
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
        entity.Property(e => e.RequestDate).HasColumnType("datetime2");
        entity.Property(e => e.DateNeeded).HasColumnType("datetime2");

        entity.Property(e => e.DraftSavedDate).HasColumnType("datetime2");
        entity.Property(e => e.LastModifiedDate).HasColumnType("datetime2");
        entity.Property(e => e.IsDraft).HasDefaultValue(false);

        entity.Property(e => e.RequesterName)
                  .HasMaxLength(100)
                  .IsRequired(false);

        entity.Property(e => e.RequesterEmail)
                  .HasMaxLength(100)
                  .IsRequired(false);

        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.Priority);
        entity.HasIndex(e => e.CreatedDate);
        entity.HasIndex(e => e.Category);
        entity.HasIndex(e => e.Department);

        entity.HasIndex(e => e.IsDraft);
        entity.HasIndex(e => new { e.CreatedBy, e.IsDraft });
        entity.HasIndex(e => new { e.IsDraft, e.Status });
        entity.HasIndex(e => e.DraftSavedDate);

        entity.HasIndex(e => e.AssignedToUserId);
        entity.HasIndex(e => e.CreatedBy);
        entity.HasIndex(e => e.RequesterEmail);
        entity.HasIndex(e => new { e.CreatedBy, e.Status });
        entity.HasIndex(e => new { e.AssignedToUserId, e.Status });
        entity.HasIndex(e => new { e.RequesterEmail, e.Status });
        entity.HasIndex(e => new { e.Status, e.CreatedDate });
      });

      // Configure TicketAttachment entity
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

        entity.HasIndex(e => e.TicketId);
        entity.HasIndex(e => e.UploadedBy);
        entity.HasIndex(e => e.UploadedDate);
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

        entity.HasIndex(e => e.TicketId);
        entity.HasIndex(e => e.CreatedBy);
        entity.HasIndex(e => e.CreatedDate);
      });

      // Configure TicketSignatory entity
      modelBuilder.Entity<TicketSignatory>(entity =>
      {
        entity.HasKey(e => e.Id);

        entity.HasOne(e => e.Ticket)
                  .WithMany(t => t.Signatories)
                  .HasForeignKey(e => e.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);

        entity.Property(e => e.CreatedDate).HasColumnType("datetime2");

        entity.HasIndex(e => e.TicketId);
      });

      // Configure DocumentRequest entity
      modelBuilder.Entity<DocumentRequest>(entity =>
      {
        entity.HasKey(e => e.Id);

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
        entity.Property(e => e.DateNeeded).HasColumnType("datetime2");

        // ADD THESE DRAFT-RELATED COLUMNS FOR DOCUMENT REQUESTS
        entity.Property(e => e.DraftSavedDate).HasColumnType("datetime2");
        entity.Property(e => e.LastModifiedDate).HasColumnType("datetime2");
        entity.Property(e => e.IsDraft).HasDefaultValue(false);

        entity.Property(e => e.RequesterName)
                  .HasMaxLength(100)
                  .IsRequired(false);

        entity.Property(e => e.RequesterEmail)
                  .HasMaxLength(100)
                  .IsRequired(false);

        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.CreatedDate);
        entity.HasIndex(e => e.Company);

        // ADD THESE NEW INDEXES FOR DRAFT FUNCTIONALITY
        entity.HasIndex(e => e.IsDraft);
        entity.HasIndex(e => new { e.CreatedBy, e.IsDraft });
        entity.HasIndex(e => new { e.IsDraft, e.Status });
        entity.HasIndex(e => e.DraftSavedDate);

        entity.HasIndex(e => e.CreatedBy);
        entity.HasIndex(e => e.RequesterEmail);
        entity.HasIndex(e => new { e.CreatedBy, e.Status });
        entity.HasIndex(e => new { e.RequesterEmail, e.Status });
        entity.HasIndex(e => new { e.Status, e.CreatedDate });
      });

      // Configure DocumentItem entity
      modelBuilder.Entity<DocumentItem>(entity =>
      {
        entity.HasKey(e => e.Id);

        entity.HasOne(e => e.DocumentRequest)
                  .WithMany(dr => dr.DocumentItems)
                  .HasForeignKey(e => e.DocumentRequestId)
                  .OnDelete(DeleteBehavior.Cascade);

        entity.Property(e => e.CreatedDate).HasColumnType("datetime2");

        entity.HasIndex(e => e.DocumentRequestId);
        entity.HasIndex(e => e.Type);
        entity.HasIndex(e => e.DocumentName);
      });

      // Configure DocumentRequestAttachment entity
      modelBuilder.Entity<DocumentRequestAttachment>(entity =>
      {
        entity.HasKey(e => e.Id);

        entity.HasOne(e => e.DocumentRequest)
                  .WithMany(dr => dr.Attachments)
                  .HasForeignKey(e => e.DocumentRequestId)
                  .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.UploadedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.UploadedBy)
                  .OnDelete(DeleteBehavior.Restrict);

        entity.Property(e => e.UploadedDate).HasColumnType("datetime2");

        entity.HasIndex(e => e.DocumentRequestId);
        entity.HasIndex(e => e.UploadedBy);
        entity.HasIndex(e => e.UploadedDate);
      });

      ConfigureStringLengths(modelBuilder);

      ConfigureDecimalPrecision(modelBuilder);
    }

    private static void ConfigureStringLengths(ModelBuilder modelBuilder)
    {
      // User entity string lengths
      modelBuilder.Entity<User>(entity =>
      {
        entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
        entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
        entity.Property(e => e.Department).HasMaxLength(100).IsRequired();
        entity.Property(e => e.Domain).HasMaxLength(50);
        entity.Property(e => e.Phone).HasMaxLength(20);
        entity.Property(e => e.Location).HasMaxLength(200);
        entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
        entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
      });

      // Ticket entity string lengths
      modelBuilder.Entity<Ticket>(entity =>
      {
        entity.Property(e => e.Subject).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Description).HasMaxLength(4000);
        entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
        entity.Property(e => e.Priority).HasMaxLength(20);
        entity.Property(e => e.Category).HasMaxLength(100);
        entity.Property(e => e.Department).HasMaxLength(100);
        entity.Property(e => e.Company).HasMaxLength(100);
        entity.Property(e => e.Location).HasMaxLength(200);
        entity.Property(e => e.AppReferenceNo).HasMaxLength(50);
        entity.Property(e => e.RequesterName).HasMaxLength(100);
        entity.Property(e => e.RequesterEmail).HasMaxLength(255);
      });

      // DocumentRequest entity string lengths
      modelBuilder.Entity<DocumentRequest>(entity =>
      {
        entity.Property(e => e.Company).HasMaxLength(100);
        entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
        entity.Property(e => e.Particulars).HasMaxLength(4000);
        entity.Property(e => e.RequesterName).HasMaxLength(100);
        entity.Property(e => e.RequesterEmail).HasMaxLength(255);
      });

      // DocumentItem entity string lengths
      modelBuilder.Entity<DocumentItem>(entity =>
      {
        entity.Property(e => e.DocumentName).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Type).HasMaxLength(100).IsRequired();
        entity.Property(e => e.Particulars).HasMaxLength(2000);
      });

      // Attachment entities string lengths
      modelBuilder.Entity<TicketAttachment>(entity =>
      {
        entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
        entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
        entity.Property(e => e.ContentType).HasMaxLength(100);
      });

      modelBuilder.Entity<DocumentRequestAttachment>(entity =>
      {
        entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
        entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
        entity.Property(e => e.ContentType).HasMaxLength(100);
      });

      // Comment entity string lengths
      modelBuilder.Entity<TicketComment>(entity =>
      {
        entity.Property(e => e.Comment).HasMaxLength(4000).IsRequired();
      });

      // Signatory entity string lengths
      modelBuilder.Entity<TicketSignatory>(entity =>
      {
        entity.Property(e => e.SignatoryName).HasMaxLength(100).IsRequired();
        entity.Property(e => e.SignatoryPosition).HasMaxLength(100).IsRequired();
      });
    }

    private static void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
    {

    }
  }
}