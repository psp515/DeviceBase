using System.ComponentModel.DataAnnotations;

namespace DeviceBaseApi.Models;

public class UserSettings
{
    [Key]
    public int SettingsId { get; set; }

    public User User { get; set; }

    public int Mode { get; set; }
    public string Language { get; set; } = string.Empty;
    public bool IsPremium { get; set; }

    public bool SoftDeleted { get; set; }

    public long EditionTics { get; set; }
    public long CreationTics { get; set; }
}

