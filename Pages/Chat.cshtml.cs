using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PingChat.Data;

namespace PingChat.Pages;

[Authorize]
public class ChatModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ChatModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public string Name { get; set; } = string.Empty;

    public string ChannelName => string.IsNullOrWhiteSpace(Name) ? "unknown" : Name;

    public string ChannelType
    {
        get
        {
            return ChannelName.ToLower() switch
            {
                "general" => "Agency + Client",
                "internal" => "Agency Only",
                "client-acme" => "Client Channel",
                "project-redesign" => "Project Room",
                _ => "Custom Channel"
            };
        }
    }

    public string ChannelDescription
    {
        get
        {
            return ChannelName.ToLower() switch
            {
                "general" => "Shared communication space for both agency and client users.",
                "internal" => "Private internal channel for agency-only discussion.",
                "client-acme" => "Dedicated client-facing room for Acme collaboration.",
                "project-redesign" => "Project-specific room for redesign updates and decisions.",
                _ => "Custom collaboration channel."
            };
        }
    }

    public List<ChatMessageItem> Messages { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Messages = await _db.ChannelMessages
            .Where(m => m.ChannelName == ChannelName)
            .OrderBy(m => m.CreatedAtUtc)
            .Select(m => new ChatMessageItem(
                m.UserName,
                m.CreatedAtUtc.ToLocalTime().ToString("h:mm tt"),
                m.Text))
            .ToListAsync();
    }

    public record ChatMessageItem(string Author, string Time, string Text);
}
