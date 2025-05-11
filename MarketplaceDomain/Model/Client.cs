using System;
using System.Collections.Generic;

namespace MarketplaceDomain.Model;

public partial class Client
{
    public int ClientId { get; set; }

    public string? Email { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
