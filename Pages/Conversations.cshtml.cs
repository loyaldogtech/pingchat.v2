using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class ConversationsModel : PageModel
{
    public string CurrentUserRole { get; private set; } = "Agency Admin";

    public List<ConversationUserItem> AvailableUsers { get; private set; } = new();

    public void OnGet()
    {
        var currentUser = User.Identity?.Name?.ToLower() ?? string.Empty;
        var isClientUser = currentUser.Contains("client");

        CurrentUserRole = isClientUser ? "Client User" : "Agency Admin";

        var allUsers = new List<ConversationUserItem>
        {
            new("amanda", "Amanda", "Agency Admin", "Direct conversation with agency leadership.", "dm-amanda"),
            new("chris", "Chris", "Project Manager", "Direct conversation for project coordination.", "dm-chris"),
            new("acme-client", "Acme Client", "Client User", "Direct client communication thread.", "dm-acme-client")
        };

        AvailableUsers = isClientUser
            ? allUsers.Where(u => u.Slug == "amanda" || u.Slug == "chris").ToList()
            : allUsers;
    }

    public record ConversationUserItem(
        string Slug,
        string DisplayName,
        string RoleLabel,
        string Description,
        string ConversationChannelName);
}
