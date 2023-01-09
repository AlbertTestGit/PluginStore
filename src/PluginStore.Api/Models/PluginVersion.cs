using System.Text.Json.Serialization;

namespace PluginStore.Api.Models;

public class PluginVersion
{
    public int PluginVersionId { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    public string FileName { get; set; }
    public string? HelpFileEn { get; set; }
    public string? HelpFileRu { get; set; }
    public string? HelpFileKz { get; set; }
    public DateTime PublicationDate { get; set; } = DateTime.UtcNow;
    public User Author { get; set; }
    public string GitLink { get; set; }
    public bool Beta { get; set; } = true;
    public DateTime? Deprecated { get; set; }
    public int PluginId { get; set; }
    [JsonIgnore]
    public Plugin Plugin { get; set; }
}