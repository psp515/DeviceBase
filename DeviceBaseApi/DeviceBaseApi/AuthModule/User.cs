using System.ComponentModel.DataAnnotations;
using DeviceBaseApi.Models;

namespace DeviceBaseApi.AuthModule;

public class User
{
    [Key]
    public int UserId { get; set; }

    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string Role { get; set; }

    public string SettingsId { get; set; }
    public UserSettings Settings { get; set; }

    public ICollection<Device> Devices { get; set; } = new List<Device>();

    public DateTime LastLogin { get; set; }
    public DateTime Edited { get; set; }
    public DateTime Created { get; set; }
}

