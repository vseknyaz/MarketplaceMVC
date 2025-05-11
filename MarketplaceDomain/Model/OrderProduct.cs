using System;
using System.Collections.Generic;

namespace MarketplaceDomain.Model;

public partial class OrderProduct
{
    public int OrderProductId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public string? AdditionalInfo { get; set; }

    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
