using System;
namespace DeviceBaseApi;



public abstract class BaseModel
{
    public BaseModel() {}

    public int Id { get; set; }
    public DateTime Edited { get; set; }
    public DateTime Created { get; set; }
}

