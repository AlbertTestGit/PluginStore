namespace PluginStore.Api.Models;

public class Plugin
{
    public int PluginId { get; set; }
    public string Name { get; set; }
    public string? DeveloperKey { get; set; }
    public string PetrelVersion { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<PluginVersion> PluginVersions { get; set; } = new List<PluginVersion>();
}