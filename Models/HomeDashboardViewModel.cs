namespace FitTrack.Models;

public class HomeDashboardViewModel
{
    public int TotalClients { get; set; }

    public int ClientsAddedThisMonth { get; set; }

    public decimal AverageClientWeight { get; set; }

    public int FitnessGoalCount { get; set; }

    public List<Client> RecentClients { get; set; } = [];
}
