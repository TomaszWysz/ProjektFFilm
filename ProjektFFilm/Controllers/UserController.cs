using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ProjektFFilm.Data;
using System.Threading.Tasks;
using System;

public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Users()
    {
        var users = _userManager.Users.ToList(); // Pobierz listę użytkowników z bazy danych
        return View(users); // Zwróć widok z listą użytkowników
    }

    [HttpPost]
    public async Task<IActionResult> Ban(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        // Banowanie użytkownika
        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

        TempData["AlertMessage"] = $"Użytkownik {user.UserName} został zbanowany.";

        return RedirectToAction("Users");
    }

    [HttpPost]
    public async Task<IActionResult> Unban(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        // Odbanowanie użytkownika
        await _userManager.SetLockoutEndDateAsync(user, null);

        TempData["AlertMessage"] = $"Użytkownik {user.UserName} został odbanowany.";

        return RedirectToAction("Users");
    }

    [HttpPost]
    public async Task<IActionResult> SetTimeout(string userId, int minutes)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var timeoutEndDate = DateTimeOffset.UtcNow.AddMinutes(minutes);

        // Ustawiamy czas blokady użytkownika
        await _userManager.SetLockoutEndDateAsync(user, timeoutEndDate);

        TempData["AlertMessage"] = $"Użytkownik {user.UserName} został zablokowany na określony czas.";

        return RedirectToAction("Users");
    }

    [HttpGet]
    public async Task<IActionResult> GetUserLockoutInfo(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user);
        string lockoutInfo = "";

        if (lockoutEndDate.HasValue && lockoutEndDate.Value > DateTimeOffset.UtcNow)
        {
            var remainingTime = lockoutEndDate.Value - DateTimeOffset.UtcNow;
            lockoutInfo = $"Użytkownik {user.UserName} jest zbanowany do {lockoutEndDate.Value.ToLocalTime()}. Pozostały czas: {remainingTime.TotalMinutes:N0} minut.";
        }
        else
        {
            lockoutInfo = $"Użytkownik {user.UserName} nie jest obecnie zbanowany.";
        }

        return Json(new { LockoutInfo = lockoutInfo });
    }


}
