﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Item
{
    public long ItemId { get; set; }
 [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    public string ItemName { get; set; } = null!;

    public long CategoryId { get; set; }

    public string? Description { get; set; }

    public long ItemTypeId { get; set; }

    public decimal Rate { get; set; }

    public int Quantity { get; set; }

    public string? ItemImage { get; set; }

    public bool? Isavailable { get; set; }

    public string Unit { get; set; } = null!;

    public bool? Isdefaulttax { get; set; }

    public decimal TaxValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool Isdelete { get; set; }

    public string ShortCode { get; set; } = null!;

    public bool IsFavourite { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Itemtype ItemType { get; set; } = null!;

    public virtual ICollection<Itemmodifiergroupmapping> Itemmodifiergroupmappings { get; } = new List<Itemmodifiergroupmapping>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; } = new List<Orderdetail>();
}
