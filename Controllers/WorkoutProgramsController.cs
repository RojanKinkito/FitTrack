using FitTrack.Data;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers;

[Authorize]
public class WorkoutProgramsController : Controller
{
    private readonly AppDbContext _context;

    public WorkoutProgramsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        var query = _context.WorkoutPrograms
            .Include(program => program.Client)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();
            query = query.Where(program =>
                program.ProgramName.Contains(searchTerm) ||
                (program.Client != null && program.Client.FullName.Contains(searchTerm)));
        }

        ViewData["SearchTerm"] = searchTerm;
        var programs = await query
            .OrderBy(program => program.ProgramName)
            .ToListAsync();

        return View(programs);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var program = await _context.WorkoutPrograms
            .Include(workoutProgram => workoutProgram.Client)
            .AsNoTracking()
            .FirstOrDefaultAsync(workoutProgram => workoutProgram.Id == id);

        return program is null ? NotFound() : View(program);
    }

    public async Task<IActionResult> Create()
    {
        await LoadClientsAsync();
        return View(new WorkoutProgram());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutProgram program)
    {
        if (!ModelState.IsValid)
        {
            await LoadClientsAsync(program.ClientId);
            return View(program);
        }

        _context.WorkoutPrograms.Add(program);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var program = await _context.WorkoutPrograms.FindAsync(id);
        if (program is null)
        {
            return NotFound();
        }

        await LoadClientsAsync(program.ClientId);
        return View(program);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkoutProgram program)
    {
        if (id != program.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadClientsAsync(program.ClientId);
            return View(program);
        }

        _context.Update(program);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.WorkoutPrograms.AnyAsync(existingProgram => existingProgram.Id == program.Id))
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

        var program = await _context.WorkoutPrograms
            .Include(workoutProgram => workoutProgram.Client)
            .AsNoTracking()
            .FirstOrDefaultAsync(workoutProgram => workoutProgram.Id == id);

        return program is null ? NotFound() : View(program);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var program = await _context.WorkoutPrograms.FindAsync(id);

        if (program is not null)
        {
            _context.WorkoutPrograms.Remove(program);
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
