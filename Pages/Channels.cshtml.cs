using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class ChannelsModel : PageModel
{
    public void OnGet()
    {
    }
}
