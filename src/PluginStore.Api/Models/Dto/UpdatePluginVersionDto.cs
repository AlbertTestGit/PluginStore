namespace PluginStore.Api.Models.Dto;

public class UpdatePluginVersionDto
{
    public int PluginVersionId { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public IFormFile? PluginFile { get; set; }
    public IFormFile? HelpFileEn { get; set; }
    public IFormFile? HelpFileRu { get; set; }
    public IFormFile? HelpFileKz { get; set; }
    public string? GitLink { get; set; }
}