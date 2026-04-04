using System;
using System.Collections.Generic;

namespace APIPSI16.Models;

public partial class Application
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? UserId { get; set; }

    public int? OpportunityId { get; set; }

    public virtual Opportunity? Opportunity { get; set; }

    public virtual User? User { get; set; }
}
