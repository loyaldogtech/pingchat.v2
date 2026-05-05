using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class ChannelsModel : PageModel
{
    public List<ChannelListItem> Channels { get; } = new()
    {
        new ChannelListItem("general", "Agency + Client", "bg-primary"),
        new ChannelListItem("internal", "Agency Only", "bg-dark"),
        new ChannelListItem("client-acme", "Client Channel", "bg-success"),
        new ChannelListItem("project-redesign", "Project Room", "bg-info text-dark")
    };

    public void OnGet()
    {
    }

    public record ChannelListItem(string Slug, string TypeLabel, string BadgeClass);
}
