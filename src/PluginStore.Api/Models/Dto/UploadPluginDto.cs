namespace PluginStore.Api.Models.Dto;

public class UploadPluginDto
{
    public string Version { get; set; }
    public string Description { get; set; }
    public IFormFile PluginFile { get; set; }
    public List<IFormFile>? HelpFiles { get; set; }
    public string GitLink { get; set; }
    public bool Beta { get; set; } = true;
    public int PluginId { get; set; }
}