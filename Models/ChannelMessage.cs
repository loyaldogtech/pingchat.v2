using System.ComponentModel.DataAnnotations;

namespace PingChat.Models;

public class ChannelMessage
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ChannelName { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
