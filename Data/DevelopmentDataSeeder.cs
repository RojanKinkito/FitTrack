using FitTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Data;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await context.Clients.AnyAsync())
        {
            return;
        }

        await using var transaction = await context.Database.BeginTransactionAsync();

        var clients = new List<Client>
        {
            new()
            {
                FullName = "Avery Morgan",
                Age = 29,
                Gender = "Female",
                Weight = 68.5m,
                Height = 168m,
                FitnessGoal = "Weight loss",
                DateJoined = new DateTime(2026, 9, 2)
            },
            new()
            {
                FullName = "Jordan Lee",
                Age = 34,
                Gender = "Male",
                Weight = 82m,
                Height = 180m,
                FitnessGoal = "Muscle gain",
                DateJoined = new DateTime(2026, 8, 15)
            },
            new()
            {
                FullName = "Casey Rivera",
                Age = 27,
                Gender = "Female",
                Weight = 61m,
                Height = 165m,
                FitnessGoal = "Strength improvement",
                DateJoined = new DateTime(2026, 10, 1)
            },
            new()
            {
                FullName = "Taylor Brooks",
                Age = 41,
                Gender = "Male",
                Weight = 91m,
                Height = 178m,
                FitnessGoal = "General fitness",
                DateJoined = new DateTime(2026, 7, 20)
            },
            new()
            {
                FullName = "Morgan Patel",
                Age = 36,
                Gender = "Female",
                Weight = 74m,
                Height = 172m,
                FitnessGoal = "Improved endurance",
                DateJoined = new DateTime(2026, 9, 18)
            }
        };

        context.Clients.AddRange(clients);
        await context.SaveChangesAsync();

        context.WorkoutPrograms.AddRange(
            new WorkoutProgram
            {
                ProgramName = "Foundations of Strength",
                Description = "A progressive full-body plan focused on building consistent strength habits.",
                DifficultyLevel = "Beginner",
                DurationWeeks = 8,
                ClientId = clients[0].Id
            },
            new WorkoutProgram
            {
                ProgramName = "Lean Muscle Builder",
                Description = "A structured resistance program with progressive overload and recovery days.",
                DifficultyLevel = "Intermediate",
                DurationWeeks = 12,
                ClientId = clients[1].Id
            });

        context.ProgressRecords.AddRange(
            new ProgressRecord
            {
                ClientId = clients[0].Id,
                Weight = 74m,
                BodyFatPercentage = 29m,
                RecordDate = new DateTime(2026, 9, 2),
                Notes = "Initial assessment"
            },
            new ProgressRecord
            {
                ClientId = clients[0].Id,
                Weight = 71m,
                BodyFatPercentage = 27.2m,
                RecordDate = new DateTime(2026, 9, 30),
                Notes = "Consistent training and nutrition plan"
            },
            new ProgressRecord
            {
                ClientId = clients[0].Id,
                Weight = 68.5m,
                BodyFatPercentage = 25.5m,
                RecordDate = new DateTime(2026, 10, 21),
                Notes = "Good progress; maintain current routine"
            },
            new ProgressRecord
            {
                ClientId = clients[1].Id,
                Weight = 79m,
                BodyFatPercentage = 18.5m,
                RecordDate = new DateTime(2026, 8, 15),
                Notes = "Initial strength baseline"
            },
            new ProgressRecord
            {
                ClientId = clients[1].Id,
                Weight = 80.5m,
                BodyFatPercentage = 18.1m,
                RecordDate = new DateTime(2026, 9, 14),
                Notes = "Increased training volume"
            },
            new ProgressRecord
            {
                ClientId = clients[1].Id,
                Weight = 82m,
                BodyFatPercentage = 17.8m,
                RecordDate = new DateTime(2026, 10, 20),
                Notes = "Steady muscle gain and strength improvements"
            });

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
