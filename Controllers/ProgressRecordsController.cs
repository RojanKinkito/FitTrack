using FitTrack.Data;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers;

[Authorize]
public class ProgressRecordsController : Controller
{
    private readonly AppDbContext _context;

    public ProgressRecordsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? clientId)
    {
        var query = _context.ProgressRecords
            .Include(record => record.Client)
            .AsNoTracking()
            .AsQueryable();

        if (clientId.HasValue)
        {
            query = query.Where(record => record.ClientId == clientId.Value);
        }

        ViewBag.Clients = new SelectList(
            await _context.Clients
                .AsNoTracking()
                .OrderBy(client => client.FullName)
                .ToListAsync(),
            nameof(Client.Id),
            nameof(Client.FullName),
            clientId);

        var records = await query
            .OrderByDescending(record => record.RecordDate)
            .ThenBy(record => record.Client!.FullName)
            .ToListAsync();

        ViewData["SelectedClientId"] = clientId;
        return View(records);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var record = await _context.ProgressRecords
            .Include(progressRecord => progressRecord.Client)
            .AsNoTracking()
            .FirstOrDefaultAsync(progressRecord => progressRecord.Id == id);

        return record is null ? NotFound() : View(record);
    }

    public async Task<IActionResult> Create(int? clientId)
    {
        await LoadClientsAsync(clientId);
        return View(new ProgressRecord
        {
            ClientId = clientId ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProgressRecord record)
    {
        if (!ModelState.IsValid)
        {
            await LoadClientsAsync(record.ClientId);
            return View(record);
        }

        _context.ProgressRecords.Add(record);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var record = await _context.ProgressRecords.FindAsync(id);
        if (record is null)
        {
            return NotFound();
        }

        await LoadClientsAsync(record.ClientId);
        return View(record);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProgressRecord record)
    {
        if (id != record.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadClientsAsync(record.ClientId);
            return View(record);
        }

        _context.Update(record);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.ProgressRecords.AnyAsync(existingRecord => existingRecord.Id == record.Id))
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

        var record = await _context.ProgressRecords
            .Include(progressRecord => progressRecord.Client)
            .AsNoTracking()
            .FirstOrDefaultAsync(progressRecord => progressRecord.Id == id);

        return record is null ? NotFound() : View(record);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var record = await _context.ProgressRecords.FindAsync(id);

        if (record is not null)
        {
            _context.ProgressRecords.Remove(record);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadClientsAsync(int? selectedClientId = null)
    {
        var clients = await _context.Clients
            .AsNoTracking()
            .OrderBy(client => client.FullName)
            .ToListAsync();

        ViewBag.Clients = new SelectList(clients, nameof(Client.Id), nameof(Client.FullName), selectedClientId);
    }
}
