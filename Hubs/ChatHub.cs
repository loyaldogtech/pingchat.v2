using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PingChat.Data;
using PingChat.Models;

namespace PingChat.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ApplicationDbContext _db;

    public ChatHub(ApplicationDbContext db)
    {
        _db = db;
    }

    private bool IsClientUser()
    {
        var email = Context.User?.Identity?.Name?.ToLower() ?? string.Empty;
        return email.Contains("client");
    }

    private bool IsBlockedChannel(string channelName)
    {
        return IsClientUser() &&
               channelName.Equals("internal", StringComparison.OrdinalIgnoreCase);
    }

    public async Task JoinChannel(string channelName)
    {
        if (string.IsNullOrWhiteSpace(channelName) || IsBlockedChannel(channelName))
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, channelName);
    }

    public async Task LeaveChannel(string channelName)
    {
        if (string.IsNullOrWhiteSpace(channelName))
        {
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);
    }

    public async Task SendMessage(string channelName, string userName, string message)
    {
        if (string.IsNullOrWhiteSpace(channelName) ||
            string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(message) ||
            IsBlockedChannel(channelName))
        {
            return;
        }

        var chatMessage = new ChannelMessage
        {
            ChannelName = channelName,
            UserName = userName,
            Text = message.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.ChannelMessages.Add(chatMessage);
        await _db.SaveChangesAsync();

        await Clients.Group(channelName).SendAsync("ReceiveMessage", userName, message.Trim());
    }
}
