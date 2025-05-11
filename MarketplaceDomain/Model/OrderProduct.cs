using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceDomain.Model;

public partial class OrderProduct
{
    public int OrderProductId { get; set; }
    [Required(ErrorMessage = "ID замовлення обов'язковий")]
    public int OrderId { get; set; }
    [Required(ErrorMessage = "ID товару обов'язковий")]
    public int ProductId { get; set; }

    public string? AdditionalInfo { get; set; }
    [Required(ErrorMessage = "Кількість обов'язкова")]
    [Range(1, int.MaxValue, ErrorMessage = "Кількість повинна бути додатньою")]
    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
