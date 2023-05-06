using System.ComponentModel.DataAnnotations;

namespace DeviceBaseApi.AuthModule.DTO;

public class RegisterRequestDTO
{
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords must match.")]
    public string ConfirmPassword { get; set; }
}