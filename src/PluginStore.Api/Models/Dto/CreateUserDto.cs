using System.ComponentModel.DataAnnotations;

namespace PluginStore.Api.Models.Dto;

public class CreateUserDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    
    [Role]
    public string? Role { get; set; } = Roles.SubsoilUser;
    public string? Password { get; set; }
}