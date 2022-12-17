namespace PluginStore.Api.Models;

public class PluginVersion
{
    public int PluginVersionId { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    //public byte[] PluginFile { get; set; }
    //public List<byte[]> HelpFiles { get; set; }
    public DateTime PublicationDate { get; set; } = DateTime.Now;
    public string Author { get; set; }
    public string GitLink { get; set; }
    public bool Beta { get; set; } = true;
    public DateTime? Deprecated { get; set; }
    public int PluginId { get; set; }
}