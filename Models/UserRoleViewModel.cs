namespace FitTrack.Models;

public class UserRoleViewModel
{
    public string UserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public IList<string> Roles { get; set; } = [];
}

public class UserManagementViewModel
{
    public IList<UserRoleViewModel> Users { get; set; } = [];
}
