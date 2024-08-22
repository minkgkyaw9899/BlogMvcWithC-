using BlogMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Db;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<BlogModel> Blogs { get; set; }
}