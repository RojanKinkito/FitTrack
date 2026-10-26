using System.ComponentModel.DataAnnotations;

namespace FitTrack.Models;

public class ProgressRecord
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a client.")]
    [Display(Name = "Client")]
    public int ClientId { get; set; }

    [Range(typeof(decimal), "1", "500", ErrorMessage = "Weight must be between 1 and 500 kg.")]
    [Display(Name = "Weight (kg)")]
    public decimal Weight { get; set; }

    [Range(typeof(decimal), "0", "100", ErrorMessage = "Body fat must be between 0% and 100%.")]
    [Display(Name = "Body fat percentage")]
    public decimal BodyFatPercentage { get; set; }

    [DataType(DataType.Date)]
    [Range(typeof(DateTime), "1900-01-01", "2100-12-31", ErrorMessage = "Please enter a valid record date.")]
    [Display(Name = "Record date")]
    public DateTime RecordDate { get; set; } = DateTime.UtcNow.Date;

    [StringLength(1000)]
    public string? Notes { get; set; }

    public Client? Client { get; set; }
}
