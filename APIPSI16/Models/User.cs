using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIPSI16.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int? Nationality { get; set; }

    public int? JobPreference { get; set; }

    // Map to DB column "Profile_Bio"
    [Column("Profile_Bio")]
    public string? ProfileBio { get; set; }

    public DateOnly? DoB { get; set; }

    public int? Role { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<CompanyMember> CompanyMembers { get; set; } = new List<CompanyMember>();

    public virtual ICollection<EmployerCandidateHistory> EmployerCandidateHistories { get; set; } = new List<EmployerCandidateHistory>();

    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}
