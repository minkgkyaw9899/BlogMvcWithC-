using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Dto;

public class BlogRequestDto
{
    [MaxLength(255)] public string Title { get; set; } = "";

    [MaxLength(255)] public string Author { get; set; } = "";

    [MaxLength(255)] public string Content { get; set; } = "";
}