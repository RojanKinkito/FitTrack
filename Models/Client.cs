using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitTrack.Models;

public class Client
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Range(1, 120)]
    public int Age { get; set; }

    [Required]
    [StringLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Range(typeof(decimal), "1", "500")]
    [Display(Name = "Weight (kg)")]
    public decimal Weight { get; set; }

    [Range(typeof(decimal), "30", "250")]
    [Display(Name = "Height (cm)")]
    public decimal Height { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Fitness goal")]
    public string FitnessGoal { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Date joined")]
    public DateTime DateJoined { get; set; } = DateTime.UtcNow.Date;

    [NotMapped]
    [Display(Name = "BMI")]
    public decimal? Bmi
    {
        get
        {
            if (Weight <= 0 || Height <= 0)
            {
                return null;
            }

            var heightInMeters = Height / 100m;
            return Math.Round(Weight / (heightInMeters * heightInMeters), 1);
        }
    }

    [NotMapped]
    public string BmiCategory
    {
        get
        {
            if (Bmi is not decimal bmi)
            {
                return "Unavailable";
            }

            return bmi switch
            {
                < 18.5m => "Underweight",
                < 25.0m => "Normal weight",
                < 30.0m => "Overweight",
                _ => "Obesity"
            };
        }
    }
}
