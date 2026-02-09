using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APIPSI16.Models;

public partial class XcleratesystemslinksSampleDbContext : DbContext
{
    public XcleratesystemslinksSampleDbContext()
    {
    }

    public XcleratesystemslinksSampleDbContext(DbContextOptions<XcleratesystemslinksSampleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<ChatUser> ChatUsers { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyMember> CompanyMembers { get; set; }

    public virtual DbSet<EmployerCandidateHistory> EmployerCandidateHistories { get; set; }

    public virtual DbSet<JobRole> JobRoles { get; set; }

    public virtual DbSet<Nationality> Nationalities { get; set; }

    public virtual DbSet<Opportunity> Opportunities { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserJobPreference> UserJobPreferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sql.bsite.net\\MSSQL2016;Database=xcleratesystemslinks_SampleDB;User Id=xcleratesystemslinks_SampleDB;Password=XcelerateDB;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC27A98CCEDF");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OpportunityId).HasColumnName("OpportunityID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Opportunity).WithMany(p => p.Applications)
                .HasForeignKey(d => d.OpportunityId)
                .HasConstraintName("FK_Applications_Opportunity");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Applications_User");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBDB08DE115");

            entity.ToTable("AuditLog");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TargetType).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLog_User");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__Chat__A9FBE7C651A747BD");

            entity.ToTable("Chat");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Chats)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Chat__CreatedByU__32E0915F");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__ChatMess__C87C0C9C7250148E");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Chat).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatMessa__ChatI__3F466844");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.SenderUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatMessa__Sende__403A8C7D");
        });

        modelBuilder.Entity<ChatUser>(entity =>
        {
            entity.HasKey(e => e.ChatUserId).HasName("PK__ChatUser__BFA9F7900B8CB40A");

            entity.HasIndex(e => new { e.ChatId, e.UserId }, "UQ_Chat_User").IsUnique();

            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("member");

            entity.HasOne(d => d.Chat).WithMany(p => p.ChatUsers)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatUsers__ChatI__38996AB5");

            entity.HasOne(d => d.User).WithMany(p => p.ChatUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatUsers__UserI__398D8EEE");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Companie__2D971CAC7600FB81");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Industry).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<CompanyMember>(entity =>
        {
            entity.HasKey(e => e.CompanyMemberId).HasName("PK__CompanyM__F1920989987E4A54");

            entity.HasIndex(e => new { e.CompanyId, e.UserId }, "UQ_Company_User").IsUnique();

            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyMembers)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyMembers_Company");

            entity.HasOne(d => d.User).WithMany(p => p.CompanyMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyMembers_User");
        });

        modelBuilder.Entity<EmployerCandidateHistory>(entity =>
        {
            entity.HasKey(e => e.EmployerCandidateHistoryId).HasName("PK__Employer__7ED6A363F8F4F90F");

            entity.ToTable("EmployerCandidateHistory");

            entity.HasIndex(e => new { e.CompanyId, e.LastContactAt }, "IX_ECH_Company_LastContact").IsDescending(false, true);

            entity.HasIndex(e => new { e.CompanyId, e.UserId }, "IX_ECH_Company_User");

            entity.HasIndex(e => e.UserId, "IX_ECH_User");

            entity.HasIndex(e => new { e.CompanyId, e.UserId, e.OpportunityId }, "UQ_EmployerCandidateHistory").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastContactAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Outcome).HasMaxLength(50);
            entity.Property(e => e.StageReached).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.EmployerCandidateHistories)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployerC__Compa__72C60C4A");

            entity.HasOne(d => d.Opportunity).WithMany(p => p.EmployerCandidateHistories)
                .HasForeignKey(d => d.OpportunityId)
                .HasConstraintName("FK__EmployerC__Oppor__74AE54BC");

            entity.HasOne(d => d.User).WithMany(p => p.EmployerCandidateHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployerC__UserI__73BA3083");
        });

        modelBuilder.Entity<JobRole>(entity =>
        {
            entity.HasKey(e => e.JobRoleId).HasName("PK__JobRoles__6D8BAC2F3B334D72");

            entity.HasIndex(e => e.Name, "UQ__JobRoles__737584F68DB4257F").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Nationality>(entity =>
        {
            entity.HasKey(e => e.NationalityId).HasName("PK__National__F628E744043E379B");

            entity.HasIndex(e => e.Name, "UQ__National__737584F6FEC464F0").IsUnique();

            entity.Property(e => e.Isocode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISOCode");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Opportunity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Oportuni__3214EC274DEA57B2");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Company).WithMany(p => p.Opportunities)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Opportunities_Company");

            entity.HasOne(d => d.Creator).WithMany(p => p.Opportunities)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("FK_Opportunities_CreatedBy");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACFE71B925");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProfileBio)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Profile_Bio");
        });

        modelBuilder.Entity<UserJobPreference>(entity =>
        {
            entity.HasKey(e => e.UserJobPreferenceId).HasName("PK__UserJobP__F82C54A77C80E128");

            entity.HasIndex(e => new { e.UserId, e.JobRoleId }, "UQ_User_JobRole").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.JobRole).WithMany(p => p.UserJobPreferences)
                .HasForeignKey(d => d.JobRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserJobPr__JobRo__02084FDA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
