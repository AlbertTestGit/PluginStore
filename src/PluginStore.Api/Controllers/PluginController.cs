using System.IO.Compression;
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

        var fileName = await SaveFile(uploadDto.PluginFile, "plugin");
        
        var userName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value!;

        var pluginVersion = new PluginVersion
        {
            Version = uploadDto.Version,
            Description = uploadDto.Description,
            FileName = fileName,
            HelpFileEn = uploadDto.HelpFileEn != null ? await SaveFile(uploadDto.HelpFileEn, "txt") : null,
            HelpFileRu = uploadDto.HelpFileRu != null ? await SaveFile(uploadDto.HelpFileRu, "txt") : null,
            HelpFileKz = uploadDto.HelpFileKz != null ? await SaveFile(uploadDto.HelpFileKz, "txt") : null,
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
        
        var uploadPath = Path.Combine(_hostEnvironment.ContentRootPath, "upload");
        
        var fileNameZip = $"{pluginVersion.Plugin.Name}-{pluginVersion.Version}.zip";

        var pluginFilePath = Path.Combine(uploadPath, pluginVersion.FileName);

        var enHelpFilePath =
            pluginVersion.HelpFileEn != null ? Path.Combine(uploadPath, pluginVersion.HelpFileEn) : null;
        var ruHelpFilePath =
            pluginVersion.HelpFileRu != null ? Path.Combine(uploadPath, pluginVersion.HelpFileRu) : null;
        var kzHelpFilePath =
            pluginVersion.HelpFileKz != null ? Path.Combine(uploadPath, pluginVersion.HelpFileKz) : null;
        
        byte[] compressedBytes;
        using (var outStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                var pluginFileInArchive = archive.CreateEntry("plugin.plugin", CompressionLevel.Optimal);
                await using (var entryStream = pluginFileInArchive.Open())
                await using (var fileToCompressStream = new FileStream(pluginFilePath, FileMode.Open, FileAccess.Read))
                {
                    await fileToCompressStream.CopyToAsync(entryStream);
                }

                if (enHelpFilePath != null)
                {
                    var enHelpFileInArchive = archive.CreateEntry("help_en.txt", CompressionLevel.Optimal);
                    await using var entryStream = enHelpFileInArchive.Open();
                    await using var fileToCompressStream = new FileStream(enHelpFilePath, FileMode.Open, FileAccess.Read);
                    await fileToCompressStream.CopyToAsync(entryStream);
                }

                if (ruHelpFilePath != null)
                {
                    var ruHelpFileInArchive = archive.CreateEntry("help_ru.txt", CompressionLevel.Optimal);
                    await using var entryStream = ruHelpFileInArchive.Open();
                    await using var fileToCompressStream = new FileStream(ruHelpFilePath, FileMode.Open, FileAccess.Read);
                    await fileToCompressStream.CopyToAsync(entryStream);
                }
                
                if (kzHelpFilePath != null)
                {
                    var kzHelpFileInArchive = archive.CreateEntry("help_kz.txt", CompressionLevel.Optimal);
                    await using var entryStream = kzHelpFileInArchive.Open();
                    await using var fileToCompressStream = new FileStream(kzHelpFilePath, FileMode.Open, FileAccess.Read);
                    await fileToCompressStream.CopyToAsync(entryStream);
                }
            }
            compressedBytes = outStream.ToArray();
        }
        return File(compressedBytes, "application/zip", fileNameZip);
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

    private async Task<string> SaveFile(IFormFile file, string extension)
    {
        var dir = Path.Combine(_hostEnvironment.ContentRootPath, "upload");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var fileName = $"{Guid.NewGuid()}.{extension}";
        
        var path = Path.Combine(dir, fileName);
            
        await using Stream stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }
}