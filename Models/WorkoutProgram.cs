using System.ComponentModel.DataAnnotations;

namespace FitTrack.Models;

public class WorkoutProgram
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Program name")]
    public string ProgramName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Difficulty level")]
    public string DifficultyLevel { get; set; } = string.Empty;

    [Range(1, 52)]
    [Display(Name = "Duration (weeks)")]
    public int DurationWeeks { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a client.")]
    [Display(Name = "Client")]
    public int ClientId { get; set; }

    public Client? Client { get; set; }
}
