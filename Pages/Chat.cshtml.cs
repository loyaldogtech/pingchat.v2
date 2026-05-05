using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class ChatModel : PageModel
{
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

    public List<ChatMessageItem> Messages { get; private set; } = new();

    public void OnGet()
    {
        Messages = new List<ChatMessageItem>
        {
            new("Amanda", "9:02 AM", $"Welcome to #{ChannelName}."),
            new("Chris", "9:04 AM", "This is where our real-time SignalR chat will appear next."),
            new("You", "9:05 AM", "Static page first. Real-time messaging after that.")
        };
    }

    public record ChatMessageItem(string Author, string Time, string Text);
}
