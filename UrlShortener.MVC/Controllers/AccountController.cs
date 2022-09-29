using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace UrlShortener.MVC.Controllers;

public class AccountController : Controller
{
    public AccountController(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
        _userManager = signInManager.UserManager;
    }

    [HttpGet]
    public IActionResult LogIn() => View();

    [HttpPost]
    public async Task<IActionResult> LogIn(LoginModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByNameAsync(model.Username)
            ?? await _userManager.FindByEmailAsync(model.Username);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User not found.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut) ModelState.AddModelError(string.Empty, "User is temporarily locked out.");
            else if (result.IsNotAllowed) ModelState.AddModelError(string.Empty, "User is not allowed to login.");
            else ModelState.AddModelError(string.Empty, "Login failed.");
            return View(model);
        }

        return RedirectToRoute(model.ReturnUrl ?? "/");
    }

    [HttpPost]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToRoute("/");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new User()
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return View(model);
        }

        await _signInManager.SignInAsync(user, model.RememberMe);

        return RedirectToRoute(model.ReturnUrl ?? "/");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var err in result.Errors) ModelState.AddModelError(string.Empty, err.Description);
            return View(model);
        }

        return RedirectToRoute("/");
    }

    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
}