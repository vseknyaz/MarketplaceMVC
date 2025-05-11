﻿using System;
using System.Collections.Generic;

namespace MarketplaceDomain.Model;

public partial class Category
{
    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
