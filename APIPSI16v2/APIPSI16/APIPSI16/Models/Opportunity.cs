using System;
using System.Collections.Generic;

namespace APIPSI16.Models;

public partial class Opportunity
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public int? CreatorId { get; set; }

    public int? CompanyId { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Company? Company { get; set; }

    public virtual User? Creator { get; set; }

    public virtual ICollection<EmployerCandidateHistory> EmployerCandidateHistories { get; set; } = new List<EmployerCandidateHistory>();
}
