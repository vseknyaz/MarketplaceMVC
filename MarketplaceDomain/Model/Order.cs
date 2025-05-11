using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceDomain.Model;

public partial class Order
{
    public int OrderId { get; set; }

    public string? OrderDate { get; set; }
    [Required(ErrorMessage = "ID клієнта обов'язковий")]
    public int? ClientId { get; set; }
    
    public int? StatusId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Statusess? Status { get; set; }
}
