using System;
using System.Collections.Generic;

namespace MarketplaceDomain.Model;

public partial class Statusess
{
    public int StatusId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
