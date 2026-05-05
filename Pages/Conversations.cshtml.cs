using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class ConversationsModel : PageModel
{
    public List<ConversationUserItem> AvailableUsers { get; private set; } = new();

    public void OnGet()
    {
        var currentUser = User.Identity?.Name?.ToLower() ?? string.Empty;
        var isClientUser = currentUser.Contains("client");

        var allUsers = new List<ConversationUserItem>
        {
            new("amanda", "Amanda", "Agency Admin", "dm-amanda"),
            new("chris", "Chris", "Project Manager", "dm-chris"),
            new("acme-client", "Acme Client", "Client User", "dm-acme-client")
        };

        AvailableUsers = isClientUser
            ? allUsers.Where(u => u.Slug == "amanda" || u.Slug == "chris").ToList()
            : allUsers;
    }

    public record ConversationUserItem(
        string Slug,
        string DisplayName,
        string RoleLabel,
        string ConversationChannelName);
}
