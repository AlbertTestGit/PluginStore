using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PluginStore.Api.Data;
using PluginStore.Api.Models;
using PluginStore.Api.Models.Dto;

namespace PluginStore.Api.Controllers;

[ApiController]
[Route("api/plugins")]
public class PluginController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public PluginController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePluginDto createPluginDto)
    {
        var plugin = new Plugin
        {
            Name = createPluginDto.Name,
            PetrelVersion = createPluginDto.PetrelVersion
        };

        _dbContext.Plugins.Add(plugin);
        await _dbContext.SaveChangesAsync();

        return Ok(plugin);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var plugins = await _dbContext.Plugins.ToListAsync();

        return Ok(plugins);
    }

    [HttpGet("{pluginId}")]
    public async Task<IActionResult> GetOne(int pluginId)
    {
        var plugin = await _dbContext.Plugins.Include(v => v.PluginVersions).FirstOrDefaultAsync(p => p.PluginId == pluginId);

        if (plugin == null)
        {
            return NotFound("Плагин не найден");
        }

        return Ok(plugin);
    }

    [HttpPost("versions/upload")]
    [Authorize]
    public async Task<IActionResult> Upload([FromForm] UploadPluginDto uploadDto)
    {
        var plugin = await _dbContext.Plugins.FirstOrDefaultAsync(p => p.PluginId == uploadDto.PluginId);

        if (plugin == null)
        {
            return BadRequest("Плагин не найден");
        }
        
        var userName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value!;

        var pluginVersion = new PluginVersion
        {
            Version = uploadDto.Version,
            Description = uploadDto.Description,
            Author = userName,
            GitLink = uploadDto.GitLink,
            Beta = uploadDto.Beta
        };
        
        plugin.PluginVersions.Add(pluginVersion);
        await _dbContext.SaveChangesAsync();

        return Ok(pluginVersion);
    }

    [HttpPost, HttpDelete]
    [Route("versions/{pluginVersionId}/beta")]
    public async Task<IActionResult> SwitchBeta(int pluginVersionId)
    {
        var pluginVersion = await _dbContext.PluginVersions.FirstOrDefaultAsync(p => p.PluginVersionId == pluginVersionId);
        
        if (pluginVersion == null)
        {
            return NotFound("Плагин не найден");
        }

        pluginVersion.Beta = Request.Method == HttpMethods.Post;
        await _dbContext.SaveChangesAsync();
        return Ok(pluginVersion);
    }
    
    [HttpPost, HttpDelete]
    [Route("versions/{pluginVersionId}/deprecated")]
    public async Task<IActionResult> SwitchDeprecated(int pluginVersionId)
    {
        var pluginVersion = await _dbContext.PluginVersions.FirstOrDefaultAsync(p => p.PluginVersionId == pluginVersionId);
        
        if (pluginVersion == null)
        {
            return NotFound("Плагин не найден");
        }

        pluginVersion.Deprecated = Request.Method == HttpMethods.Post ? DateTime.Now : null;
        await _dbContext.SaveChangesAsync();
        return Ok(pluginVersion);
    }

    [HttpGet("versions/{pluginId}/current")]
    public async Task<IActionResult> CurrentVersion(int pluginId)
    {
        var plugin = await _dbContext.Plugins.Include(v => v.PluginVersions).FirstOrDefaultAsync(p => p.PluginId == pluginId);

        if (plugin == null)
        {
            return NotFound("Плагин не найден");
        }

        var currentVersion = plugin.PluginVersions.LastOrDefault();

        return Ok(currentVersion);
    }
}