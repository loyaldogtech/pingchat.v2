using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PingChat.Pages;

[Authorize]
public class ChatModel : PageModel
{
    public void OnGet()
    {
    }
}
