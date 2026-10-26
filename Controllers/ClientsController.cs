using FitTrack.Data;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers;

[Authorize]
public class ClientsController : Controller
{
    private readonly AppDbContext _context;

    public ClientsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? gender)
    {
        searchTerm = searchTerm?.Trim();
        gender = gender?.Trim();

        var query = _context.Clients
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchPattern = $"%{searchTerm}%";
            query = query.Where(client =>
                EF.Functions.Like(client.FullName, searchPattern) ||
                EF.Functions.Like(client.Gender, searchPattern) ||
                EF.Functions.Like(client.FitnessGoal, searchPattern));
        }

        if (!string.IsNullOrWhiteSpace(gender))
        {
            query = query.Where(client => client.Gender == gender);
        }

        var genders = await _context.Clients
            .AsNoTracking()
            .Where(client => client.Gender != string.Empty)
            .Select(client => client.Gender)
            .Distinct()
            .OrderBy(value => value)
            .ToListAsync();

        ViewBag.Genders = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(genders, gender);
        ViewData["SearchTerm"] = searchTerm;
        ViewData["SelectedGender"] = gender;

        var clients = await query
            .OrderByDescending(client => client.DateJoined)
            .ThenBy(client => client.FullName)
            .ToListAsync();

        return View(clients);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var client = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id);

        if (client is null)
        {
            return NotFound();
        }

        ViewBag.ProgressRecords = await _context.ProgressRecords
            .Where(record => record.ClientId == client.Id)
            .AsNoTracking()
            .OrderBy(record => record.RecordDate)
            .ThenBy(record => record.Id)
            .ToListAsync();

        return View(client);
    }

    public IActionResult Create()
    {
        return View(new Client());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Client client)
    {
        if (!ModelState.IsValid)
        {
            return View(client);
        }

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var client = await _context.Clients.FindAsync(id);
        return client is null ? NotFound() : View(client);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Client client)
    {
        if (id != client.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(client);
        }

        _context.Update(client);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Clients.AnyAsync(existingClient => existingClient.Id == client.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var client = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id);

        return client is null ? NotFound() : View(client);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client is not null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
