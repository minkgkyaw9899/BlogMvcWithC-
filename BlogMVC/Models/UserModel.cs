using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogMVC.Models;

[Table("Tbl_User")]
public class UserModel
{
    [Key] public int UserId { get; set; }
    
    [MaxLength(60)] public string Name { get; set; } = "";
    
    [MaxLength(60)] public string Email { get; set; } = "";
    
    [MaxLength(60)] public string Password { get; set; } = "";
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}