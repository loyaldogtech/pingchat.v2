using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    public string WorkspaceName { get; private set; } = "Acme Agency";

    public string CurrentUserRole { get; private set; } = "Agency Admin";

    public int ChannelCount { get; private set; } = 4;

    public int ClientCount { get; private set; } = 1;

    public void OnGet()
    {
        var email = User.Identity?.Name?.ToLower() ?? string.Empty;
        var isClientUser = email.Contains("client");

        CurrentUserRole = isClientUser ? "Client User" : "Agency Admin";
        ChannelCount = isClientUser ? 3 : 4;
    }
}
