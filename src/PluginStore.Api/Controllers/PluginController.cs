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
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IConfiguration _configuration;

    public PluginController(ApplicationDbContext dbContext, IMapper mapper, IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _hostEnvironment = hostEnvironment;
        _configuration = configuration;
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

    [HttpPost("versions/upload")]
    [Authorize(Roles = $"{Roles.Developer}, {Roles.Administrator}")]
    public async Task<IActionResult> Upload([FromForm] UploadPluginDto uploadDto)
    {
        var plugin = await _dbContext.Plugins.FirstOrDefaultAsync(p => p.PluginId == uploadDto.PluginId);

        if (plugin == null)
        {
            return BadRequest("Плагин не найден");
        }

        var fileName = await SaveFile(uploadDto.PluginFile);
        
        var userName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value!;

        var pluginVersion = new PluginVersion
        {
            Version = uploadDto.Version,
            Description = uploadDto.Description,
            FileName = fileName,
            Author = userName,
            GitLink = uploadDto.GitLink,
            Beta = uploadDto.Beta
        };
        
        plugin.PluginVersions.Add(pluginVersion);
        await _dbContext.SaveChangesAsync();

        var dto = _mapper.Map<PluginVersionDto>(pluginVersion);
        return Ok(dto);
    }

    [HttpGet("dl/{pluginVersionId}")]
    public async Task<IActionResult> Download(int pluginVersionId)
    {
        var pluginVersion = await _dbContext.PluginVersions.Include(p => p.Plugin).FirstOrDefaultAsync(p => p.PluginVersionId == pluginVersionId);

        if (pluginVersion == null)
        {
            return NotFound("Плагин не найден");
        }
        
        var path = Path.Combine(_hostEnvironment.ContentRootPath, "upload", pluginVersion.FileName);
        var fileName = $"{pluginVersion.Plugin.Name}-{pluginVersion.Version}.zip";

        var content = new FileStream(path, FileMode.Open, FileAccess.Read);
        return File(content, "application/octet-stream", fileName);
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

        pluginVersion.Deprecated = Request.Method == HttpMethods.Post ? DateTime.UtcNow : null;
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

    private async Task<string> SaveFile(IFormFile file)
    {
        var dir = Path.Combine(_hostEnvironment.ContentRootPath, "upload");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var fileName = $"{Guid.NewGuid()}.zip";
        
        var path = Path.Combine(dir, fileName);
            
        await using Stream stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }
}