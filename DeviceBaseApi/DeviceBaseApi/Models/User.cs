using System.ComponentModel.DataAnnotations;

namespace DeviceBaseApi.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

	public string Email { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }

    public UserSettings Settings { get; set; }

    public ICollection<Device> Devices { get; set; } = new List<Device>();

    public bool SoftDeleted { get; set; }

    public long EditionTics { get; set; }
    public long CreationTics { get; set; }
}

