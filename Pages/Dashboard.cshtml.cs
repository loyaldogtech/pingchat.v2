using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    public string WorkspaceName { get; } = "Acme Agency";

    public string CurrentUserRole { get; } = "Agency Admin";

    public int ChannelCount { get; } = 4;

    public int ClientCount { get; } = 1;

    public void OnGet()
    {
    }
}
