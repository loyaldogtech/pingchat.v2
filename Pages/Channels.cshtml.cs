using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class ChannelsModel : PageModel
{
    public string WorkspaceName { get; } = "Acme Agency";

    public string CurrentUserRole { get; } = "Agency Admin";

    public List<ChannelListItem> Channels { get; private set; } = new();

    public void OnGet()
    {
        Channels = new List<ChannelListItem>
        {
            new("general", "Agency + Client", "Shared communication space for both agency and client users.", "bg-primary"),
            new("internal", "Agency Only", "Private internal channel for agency-only discussion.", "bg-dark"),
            new("client-acme", "Client Channel", "Dedicated client-facing room for Acme collaboration.", "bg-success"),
            new("project-redesign", "Project Room", "Project-specific room for redesign updates and decisions.", "bg-info text-dark")
        };
    }

    public record ChannelListItem(
        string Slug,
        string TypeLabel,
        string Description,
        string BadgeClass);
}
