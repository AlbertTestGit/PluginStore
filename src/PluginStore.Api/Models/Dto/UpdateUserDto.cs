namespace PluginStore.Api.Models.Dto;

public class UpdateUserDto
{
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? LicenseNumber { get; set; }
    
    [Role]
    public string? Role { get; set; }
    public string? Password { get; set; }
}