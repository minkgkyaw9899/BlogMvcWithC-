using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogMVC.Models;

[Table("Tbl_Blog")]
public class BlogModel
{
    [Key] public int BlogId { get; set; }

    [MaxLength(255)] public string Title { get; set; } = "";

    [MaxLength(255)] public string Author { get; set; } = "";

    [MaxLength(1000)] public string Content { get; set; } = "";
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}