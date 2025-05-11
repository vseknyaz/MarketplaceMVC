using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceDomain.Model;

public partial class Category
{
    
    [Required(ErrorMessage = "Назва категорії обов'язкова")]
    [StringLength(50, ErrorMessage = "Назва категорії не може бути довшою за 50 символів")]
    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
