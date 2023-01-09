namespace PluginStore.Api.Models.Dto;

public class UpdatePluginDto
{
    public int PluginId { get; set; }
    public string? Name { get; set; }
    public string? PetrelVersion { get; set; }
    public string? DeveloperKey { get; set; }
}