using System.ComponentModel.DataAnnotations;

namespace PluginStore.Api.Models.Dto;

public class RegisterDto
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string? LicenseNumber { get; set; }
}