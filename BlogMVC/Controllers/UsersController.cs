using System.Security.Claims;
using BlogMVC.Db;
using BlogMVC.Dto;
using BlogMVC.Models;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Controllers;

public class UsersController : Controller
{
    private readonly AppDbContext _db;

    private readonly ILogger<UsersController> _logger;

    public UsersController(AppDbContext db, ILogger<UsersController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [ActionName("Register")]
    public IActionResult Register()
    {
        _logger.LogInformation("Info ::::::  View register user page");
        return View("Register");
    }

    [HttpPost()]
    [ActionName("Register")]
    public async Task<IActionResult> CreateUser(RegisterRequestDto requestDto)
    {
        _logger.LogInformation("Info :::::: Create new user");

        // find user is existed or not
        var existedUser = await _db.Users.FirstOrDefaultAsync(x => x.Email == requestDto.Email);
        if (existedUser != null)
        {
            _logger.LogError("Error :::::: Create new user but user already exited!");
            return View("Register", new
            {
                AlreadyExisted = true,
            });
        }

        var isMatch = requestDto.Password == requestDto.ConfirmPassword;

        if (!isMatch)
        {
            _logger.LogWarning("Warning :::::: Create new user but invalid confirm password!");
            return View("Register", new
            {
                InvalidConfirmPassword = true,
            });
        }

        var password = Argon2.Hash(requestDto.Password);

        var userData = new UserModel()
        {
            Email = requestDto.Email,
            Name = requestDto.Name,
            Password = password,
        };

        await _db.AddAsync(userData);

        var result = await _db.SaveChangesAsync();

        if (result <= 0)
        {
            _logger.LogError("Error :::::: Create new user failed!");
            return View("Register", new
            {
                CreateUserError = true,
            });
        }

        _logger.LogInformation("Success :::::: Create new user successfully!");
        return Redirect("/Home");
    }

    [ActionName("Login")]
    public IActionResult Login()
    {
        _logger.LogInformation("Info ::::::  View Login page");
        return View("Login");
    }

    [HttpPost()]
    [ActionName("Login")]
    public async Task<IActionResult> LoginUser(LoginRequestDto requestDto)
    {
        _logger.LogInformation("Info :::::: Login user");

        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == requestDto.Email);

        if (user is null)
        {
            _logger.LogError("Error :::::: Login user but user not exited!");
            return View("Login", new
            {
                AlreadyExisted = true,
            });
        }

        var isMatchPwd = Argon2.Verify(user.Password, requestDto.Password);

        if (!isMatchPwd)
        {
            _logger.LogWarning("Warning :::::: Login user with wrong password!");
            return View("Login", new
            {
                InvalidUser = true,
            });
        }

        // var userData = new UserModel()
        // {
        //     Name = user.Name,
        //     Email = user.Email,
        //     UserId = user.UserId,
        // };

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, "User"),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(principal);

        _logger.LogInformation("Success :::::: Login user success!");
        return Redirect("/Home");
    }

    [Authorize(Roles = "User")]
    [ActionName("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        _logger.LogInformation("Success :::::: Logout user success!");
        return Redirect("/");
    }

    [Authorize(Roles = "User")]
    [ActionName("Profile")]
    public async Task<IActionResult> UserProfile()
    {
        _logger.LogInformation("Info :::::: View user profile page");
        
        var email = HttpContext.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null)
        {
            _logger.LogError("Error :::::: User not found, redirect to login");
            return Redirect("/Login");
        }

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
        
        if (user is null)
        {
            _logger.LogError("Error :::::: User not found, redirect to Home page");
            return Redirect("/");
        }
        
        _logger.LogInformation("Success :::::: View user profile page successfully!");
        return View("UserProfile", user);
    }
    

}