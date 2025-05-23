﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceDomain.Model;

public partial class Client
{
    public int ClientId { get; set; }

    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Ім'я обов'язкове")]
    [StringLength(100, ErrorMessage = "Ім'я не може бути довшим за 100 символів")]
    public string FullName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
