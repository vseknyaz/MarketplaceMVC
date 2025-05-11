using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceDomain.Model;

public partial class Product
{
    public int ProductId { get; set; }
    [Required(ErrorMessage = "ID магазину обов'язковий")]
    public int ShopId { get; set; }
    [Required(ErrorMessage = "Ім'я категорії обов'язковий")]
    public string CategoryId { get; set; } = null!;
    [Required(ErrorMessage = "Назва товару обов'язкова")]
    [StringLength(100, ErrorMessage = "Назва не може бути довшою за 100 символів")]
    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }
    [Required(ErrorMessage = "Ціна обов'язкова")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути додатньою")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Запас обов'язковий")]
    [Range(0, int.MaxValue, ErrorMessage = "Запас не може бути від'ємним")]
    public string Stock { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Shop Shop { get; set; } = null!;
}
