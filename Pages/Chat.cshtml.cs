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

    public bool IsDirectMessage => ChannelName.StartsWith("dm-", StringComparison.OrdinalIgnoreCase);

    public string DisplayChannelTitle
    {
        get
        {
            return ChannelName.ToLower() switch
            {
                "dm-amanda" => "Amanda",
                "dm-chris" => "Chris",
                "dm-acme-client" => "Acme Client",
                _ => ChannelName
            };
        }
    }

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
                "dm-amanda" => "Direct Message",
                "dm-chris" => "Direct Message",
                "dm-acme-client" => "Direct Message",
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
                "dm-amanda" => "Private direct conversation with Amanda.",
                "dm-chris" => "Private direct conversation with Chris.",
                "dm-acme-client" => "Private direct conversation with the Acme client.",
                _ => "Custom collaboration channel."
            };
        }
    }

    public List<ChatMessageItem> Messages { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (UserBlockedFromChannel(ChannelName))
        {
            return RedirectToPage("/Channels");
        }

        await LoadMessagesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int messageId, string name)
    {
        if (UserBlockedFromChannel(name))
        {
            return RedirectToPage("/Channels");
        }

        var currentUserName = User.Identity?.Name ?? string.Empty;

        var message = await _db.ChannelMessages.FirstOrDefaultAsync(m =>
            m.Id == messageId &&
            m.ChannelName == name &&
            m.UserName == currentUserName);

        if (message is not null)
        {
            _db.ChannelMessages.Remove(message);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { name });
    }

    private bool UserBlockedFromChannel(string channelName)
    {
        var email = User.Identity?.Name?.ToLower() ?? string.Empty;
        var isClientUser = email.Contains("client");

        return isClientUser &&
               channelName.Equals("internal", StringComparison.OrdinalIgnoreCase);
    }

    private async Task LoadMessagesAsync()
    {
        var currentUserName = User.Identity?.Name ?? string.Empty;

        Messages = await _db.ChannelMessages
            .Where(m => m.ChannelName == ChannelName)
            .OrderBy(m => m.CreatedAtUtc)
            .Select(m => new ChatMessageItem(
                m.Id,
                m.UserName,
                m.CreatedAtUtc.ToLocalTime().ToString("h:mm tt"),
                m.Text,
                m.UserName == currentUserName))
            .ToListAsync();
    }

    public record ChatMessageItem(
        int Id,
        string Author,
        string Time,
        string Text,
        bool CanDelete);
}
