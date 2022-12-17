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
        var plugins = await _dbContext.Plugins.Include(v => v.PluginVersions).ToListAsync();

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
}