using System.IO.Compression;
using System.Security.Claims;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public PluginController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Developer}, {Roles.Administrator}")]
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

    [HttpPut]
    [Authorize(Roles = $"{Roles.Developer}, {Roles.Administrator}")]
    public async Task<IActionResult> Update([FromBody] UpdatePluginDto updatePluginDto)
    {
        var plugin = await _dbContext.Plugins.FirstOrDefaultAsync(p => p.PluginId == updatePluginDto.PluginId);

        if (plugin == null)
        {
            return NotFound("Плагин не найден");
        }

        plugin.Name = updatePluginDto.Name ?? plugin.Name;
        plugin.DeveloperKey = updatePluginDto.DeveloperKey ?? plugin.DeveloperKey;
        plugin.PetrelVersion = updatePluginDto.PetrelVersion ?? plugin.PetrelVersion;

        _dbContext.Plugins.Update(plugin);
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

        return Ok(_mapper.Map<PluginDto>(plugin));
    }

    [HttpGet("versions/{pluginId}/current")]
    public async Task<IActionResult> CurrentVersion(int pluginId)
    {
        var plugin = await _dbContext.Plugins.Include(v => v.PluginVersions).FirstOrDefaultAsync(p => p.PluginId == pluginId);

        if (plugin == null)
        {
            return NotFound("Плагин не найден");
        }

        // TODO: нужно учитовать beta и deprecated
        var currentVersion = plugin.PluginVersions.LastOrDefault();

        return Ok(currentVersion);
    }
}