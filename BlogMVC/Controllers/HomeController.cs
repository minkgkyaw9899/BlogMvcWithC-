using System.Diagnostics;
using BlogMVC.Db;
using Microsoft.AspNetCore.Mvc;
using BlogMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext db, ILogger<HomeController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Log ::::: All blogs action executed");
        var blogs = await _db.Blogs.AsNoTracking().OrderByDescending(i => i.BlogId).ToListAsync();

        return View("Index", blogs);
    }
}