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

    public void OnGet()
    {
    }
}
