using System.Diagnostics;
using BlogMVC.Db;
using Microsoft.AspNetCore.Mvc;
using BlogMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogMVC.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(AppDbContext db, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Log ::::: All blogs action executed");
        List<BlogModel> blogs = new List<BlogModel>();
        var item = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
        if (item is not null && item.Value == "User")
        {
            blogs = await _db.Blogs
                .AsNoTracking()
                .OrderByDescending(i => i.BlogId)
                .ToListAsync();
        }

        return View("Index", blogs);
    }
}