﻿using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class DrinkType
{
    public int DrinkTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public string? ImagePath { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
