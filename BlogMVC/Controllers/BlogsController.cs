using BlogMVC.Db;
using BlogMVC.Dto;
using BlogMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogMVC.Controllers;

[Authorize(Roles = "User,Manager")]
public class BlogsController: Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<UsersController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BlogsController(AppDbContext db, ILogger<UsersController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    [ActionName("Create")]
    public IActionResult Index()
    {
        _logger.LogInformation("Info ::::::  View create blog page");
        return View("Create");
    }

    [HttpPost]
    [ActionName("Create")]
    public async Task<IActionResult> CreateBlog(BlogRequestDto requestDto)
    {
        _logger.LogInformation("Info :::::: Create new blog");

        string userId = _httpContextAccessor.HttpContext!.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;


        await _db.Blogs.AddAsync(new BlogModel()
        {
            Author = requestDto.Author,
            Content = requestDto.Content,
            Title = requestDto.Title,
        });

        var result = await _db.SaveChangesAsync();
        
        if (result <= 0)
        {
            _logger.LogError("Error :::::: Create new blog failed!");
            return View("Create", new
            {
                
                CreateUserError = true,
            });
        }
            
        _logger.LogInformation("Success :::::: Create new blog successfully!");
        
        return Redirect("/");
    }

    [ActionName("Detail")]
    public async Task<IActionResult> BlogDetail(int id)
    {
        _logger.LogInformation("Info ::::: View blog detail page ==> id={id}", id);
        
        var blog = await _db.Blogs.AsNoTracking().FirstOrDefaultAsync(x => x.BlogId == id);

        if (blog is null)
        {
            _logger.LogError("Error ::::: Blog not found ==> id={id}", id);
            return View("Error");
        }
        
        _logger.LogInformation("Success ::::: Blog found ==> id={id}", id);
        return View("BlogDetail", blog);
    }
    
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteBlog(int id)
    {
        _logger.LogInformation("Info ::::: View delete blog page ==> id={id}", id);
        
        var blog = await _db.Blogs.AsNoTracking().FirstOrDefaultAsync(item => item.BlogId == id);

        if (blog is null)
        {
            _logger.LogError("Error ::::: Blog not found ==> id={id}", id);
            return Redirect("/");
        }

        _db.Blogs.Remove(blog);

        await _db.SaveChangesAsync();

        _logger.LogInformation("Success ::::: Delete blog ==> id={id}", id);
        return Redirect("");
    }
    
    [ActionName("Edit")]
    public async Task<IActionResult> EditBlog(int id)
    {
        _logger.LogInformation("Info ::::: View edit blog page ==> id={id}", id);

        var blog = await _db.Blogs.AsNoTracking().FirstOrDefaultAsync(item => item.BlogId == id);

        if (blog is null)
        {
            _logger.LogError("Error ::::: Blog not found ==> id={id}", id);
            return Redirect("/");
        }

        _logger.LogInformation("Success ::::: Blog found ==> id={id}", id);
        return View("EditBlog", blog);
    }
    
    [HttpPost]
    [ActionName("Edit")]
    public async Task<IActionResult> EditBlog(int id, BlogRequestDto requestDto)
    {
        _logger.LogInformation("Info ::::: Edit blog ==> id={id}", id);

        var blog = await _db.Blogs.AsNoTracking().FirstOrDefaultAsync(item => item.BlogId == id);

        if (blog is null)
        {
            _logger.LogError("Error ::::: Blog not found ==> id={id}", id);
            return Redirect("/");
        }

        blog.Author = requestDto.Author;
        blog.Content = requestDto.Content;
        blog.Title = requestDto.Title;

        _db.Entry(blog).State = EntityState.Modified;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Success ::::: Edit blog successfully ==> id={id}", id);
        return Redirect("/");
    }
}