using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers;

[Authorize(Roles = "Admin")]
public class UserManagementController : Controller
{
    private static readonly HashSet<string> ManagedRoles = ["Admin", "Trainer"];
    private readonly UserManager<IdentityUser> _userManager;

    public UserManagementController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderBy(user => user.Email)
            .ToListAsync();

        var userRows = new List<UserRoleViewModel>();
        foreach (var user in users)
        {
            userRows.Add(new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? user.UserName ?? "Unknown user",
                Roles = await _userManager.GetRolesAsync(user)
            });
        }

        return View(new UserManagementViewModel { Users = userRows });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(string userId, string role, bool assign)
    {
        if (!ManagedRoles.Contains(role))
        {
            return BadRequest();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        if (!assign && role == "Admin" && userId == _userManager.GetUserId(User))
        {
            TempData["UserManagementError"] = "You cannot remove your own Admin role.";
            return RedirectToAction(nameof(Index));
        }

        if (!assign && role == "Admin")
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (admins.Count <= 1)
            {
                TempData["UserManagementError"] = "The last Admin role cannot be removed.";
                return RedirectToAction(nameof(Index));
            }
        }

        var result = assign
            ? await _userManager.AddToRoleAsync(user, role)
            : await _userManager.RemoveFromRoleAsync(user, role);

        if (!result.Succeeded)
        {
            TempData["UserManagementError"] = string.Join("; ", result.Errors.Select(error => error.Description));
        }

        return RedirectToAction(nameof(Index));
    }
}
