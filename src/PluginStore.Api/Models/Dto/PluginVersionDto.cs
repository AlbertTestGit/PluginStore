namespace PluginStore.Api.Models.Dto;

public class PluginVersionDto
{
    public int PluginVersionId { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    // TODO: нужно придумать другой способ генерации ссылки
    public string FileUrl => $"http://localhost:5100/api/plugin-versions/download/{PluginVersionId}";
    public DateTime PublicationDate { get; set; } = DateTime.Now;
    public string Author { get; set; }
    public string GitLink { get; set; }
    public bool Beta { get; set; }
    public DateTime? Deprecated { get; set; }
    public int PluginId { get; set; }
}