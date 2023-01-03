namespace PluginStore.Api.Models.Dto;

public class PluginDto
{
    public int PluginId { get; set; }
    public string Name { get; set; }
    public string? DeveloperKey { get; set; }
    public string PetrelVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PluginVersionDto> PluginVersions { get; set; }
}