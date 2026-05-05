using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class ChannelsModel : PageModel
{
    public string WorkspaceName { get; private set; } = "Acme Agency";

    public string CurrentUserRole { get; private set; } = "Agency Admin";

    public List<ChannelListItem> Channels { get; private set; } = new();

    public void OnGet()
    {
        var email = User.Identity?.Name?.ToLower() ?? string.Empty;
        var isClientUser = email.Contains("client");

        CurrentUserRole = isClientUser ? "Client User" : "Agency Admin";

        var allChannels = new List<ChannelListItem>
        {
            new("general", "Agency + Client", "Shared communication space for both agency and client users.", "bg-primary"),
            new("internal", "Agency Only", "Private internal channel for agency-only discussion.", "bg-dark"),
            new("client-acme", "Client Channel", "Dedicated client-facing room for Acme collaboration.", "bg-success"),
            new("project-redesign", "Project Room", "Project-specific room for redesign updates and decisions.", "bg-info text-dark")
        };

        Channels = isClientUser
            ? allChannels.Where(c => c.Slug != "internal").ToList()
            : allChannels;
    }

    public record ChannelListItem(
        string Slug,
        string TypeLabel,
        string Description,
        string BadgeClass);
}
