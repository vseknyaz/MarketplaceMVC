using System;
using System.Collections.Generic;

namespace MarketplaceDomain.Model;

public partial class Product
{
    public int ProductId { get; set; }

    public int ShopId { get; set; }

    public string CategoryId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? Stock { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Shop Shop { get; set; } = null!;
}
