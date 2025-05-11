using System;
using System.Collections.Generic;

namespace MarketplaceDomain.Model;

public partial class Shop
{
    public int ShopId { get; set; }

    public string ShopName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
