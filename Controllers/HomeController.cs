using System.Diagnostics;
using FitTrack.Data;
using FitTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return View("Landing");
            }

            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var nextMonth = monthStart.AddMonths(1);
            var weights = await _context.Clients
                .Select(client => client.Weight)
                .ToListAsync();

            var dashboard = new HomeDashboardViewModel
            {
                TotalClients = await _context.Clients.CountAsync(),
                ClientsAddedThisMonth = await _context.Clients
                    .CountAsync(client => client.DateJoined >= monthStart && client.DateJoined < nextMonth),
                AverageClientWeight = weights.Count == 0 ? 0m : weights.Average(),
                FitnessGoalCount = await _context.Clients
                    .Where(client => client.FitnessGoal != string.Empty)
                    .Select(client => client.FitnessGoal)
                    .Distinct()
                    .CountAsync(),
                RecentClients = await _context.Clients
                    .AsNoTracking()
                    .OrderByDescending(client => client.DateJoined)
                    .ThenByDescending(client => client.Id)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboard);
        }

        [AllowAnonymous]
        public IActionResult Landing()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
