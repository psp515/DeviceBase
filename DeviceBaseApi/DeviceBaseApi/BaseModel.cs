using System.ComponentModel.DataAnnotations;

namespace DeviceBaseApi;

public abstract class BaseModel
{
    [Key]
    public int Id { get; set; }
    public DateTime Edited { get; set; }
    public DateTime Created { get; set; }
}

