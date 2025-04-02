using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Category
{
    public long CategoryId { get; set; }

    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Category Name cannot exceed 50 characters.")]
    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool Isdelete { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Item> Items { get; } = new List<Item>();

    public virtual User? ModifiedByNavigation { get; set; }
}
