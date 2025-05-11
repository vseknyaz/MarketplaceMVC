using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceDomain.Model;

public partial class Shop
{
    public int ShopId { get; set; }
    [Required(ErrorMessage = "Назва магазину обов'язкова")]
    [StringLength(100, ErrorMessage = "Назва не може бути довшою за 100 символів")]
    public string ShopName { get; set; } = null!;
    [Required(ErrorMessage = "Адреса обов'язкова")]
    [StringLength(200, ErrorMessage = "Адреса не може бути довшою за 200 символів")]
    public string Address { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
