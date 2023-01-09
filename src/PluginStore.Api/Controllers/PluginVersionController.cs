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
[Route("api/plugin-versions")]
public class PluginVersionController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHostEnvironment _hostEnvironment;

    public PluginVersionController(ApplicationDbContext dbContext, IMapper mapper, IHostEnvironment hostEnvironment)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _hostEnvironment = hostEnvironment;
    }

    [HttpGet("{pluginVersionId}")]
    public async Task<IActionResult> GetOne(int pluginVersionId)
    {
        var pluginVersion =
            await _dbContext.PluginVersions.FirstOrDefaultAsync(p => p.PluginVersionId == pluginVersionId);

        if (pluginVersion == null)
        {
            return NotFound("Плагин не найден");
        }

        // TODO: Проверить маппинг
        return Ok(_mapper.Map<PluginVersionDto>(pluginVersion));
    }

    [HttpGet("versions/{pluginId}/current")]
    public async Task<IActionResult> CurrentVersion(int pluginId)
    {
        var plugin = await _dbContext.Plugins.Include(v => v.PluginVersions).FirstOrDefaultAsync(p => p.PluginId == pluginId);

        if (plugin == null)
        {
            return NotFound("Плагин не найден");
        }
        
        var currentVersion = plugin.PluginVersions.Where(p => (p.Deprecated == null) && (p.Beta == false)).LastOrDefault();

        return Ok(currentVersion);
    }

    [HttpPost("upload")]
    [Authorize(Roles = $"{Roles.Developer}, {Roles.Administrator}")]
    public async Task<IActionResult> Upload([FromForm] UploadPluginDto uploadDto)
    {
        var plugin = await _dbContext.Plugins.FirstOrDefaultAsync(p => p.PluginId == uploadDto.PluginId);

        if (plugin == null)
        {
            return BadRequest("Плагин не найден");
        }

        var fileName = await SaveFile(uploadDto.PluginFile, "plugin");
        
        var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        var pluginVersion = new PluginVersion
        {
            Version = uploadDto.Version,
            Description = uploadDto.Description,
            FileName = fileName,
            HelpFileEn = uploadDto.HelpFileEn != null ? await SaveFile(uploadDto.HelpFileEn, "txt") : null,
            HelpFileRu = uploadDto.HelpFileRu != null ? await SaveFile(uploadDto.HelpFileRu, "txt") : null,
            HelpFileKz = uploadDto.HelpFileKz != null ? await SaveFile(uploadDto.HelpFileKz, "txt") : null,
            Author = user!,
            GitLink = uploadDto.GitLink,
            Beta = uploadDto.Beta
        };
        
        plugin.PluginVersions.Add(pluginVersion);
        await _dbContext.SaveChangesAsync();

        var dto = _mapper.Map<PluginVersionDto>(pluginVersion);
        return Ok(dto);
    }
    
    [HttpGet("download/{pluginVersionId}")]
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
    
    [HttpPut("update")]
    [Authorize(Roles = $"{Roles.Developer}, {Roles.Administrator}")]
    public async Task<IActionResult> UpdatePluginVersion([FromForm] UpdatePluginVersionDto updatePluginVersionDto)
    {
        var pluginVersion =
            await _dbContext.PluginVersions.Include(u => u.Author).FirstOrDefaultAsync(p =>
                p.PluginVersionId == updatePluginVersionDto.PluginVersionId);

        if (pluginVersion == null)
        {
            return NotFound("Плагин не найден");
        }
        
        var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!);
        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!;
        
        if (pluginVersion.Author.UserId != userId && userRole != Roles.Administrator)
        {
            return Forbid();
        }

        pluginVersion.Version = updatePluginVersionDto.Version ?? pluginVersion.Version;
        pluginVersion.Description = updatePluginVersionDto.Description ?? pluginVersion.Description;
        pluginVersion.GitLink = updatePluginVersionDto.GitLink ?? pluginVersion.GitLink;

        if (updatePluginVersionDto.PluginFile != null)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "upload", pluginVersion.FileName);
            
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var fileName = await SaveFile(updatePluginVersionDto.PluginFile, "plugin");
            pluginVersion.FileName = fileName;
        }

        if (updatePluginVersionDto.HelpFileEn != null)
        {
            if (pluginVersion.HelpFileEn != null)
            {
                var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "upload", pluginVersion.HelpFileEn);
                
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            
            var fileName = await SaveFile(updatePluginVersionDto.HelpFileEn, "txt");
            pluginVersion.HelpFileEn = fileName;
        }
        
        if (updatePluginVersionDto.HelpFileRu != null)
        {
            if (pluginVersion.HelpFileRu != null)
            {
                var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "upload", pluginVersion.HelpFileRu);
                
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            
            var fileName = await SaveFile(updatePluginVersionDto.HelpFileRu, "txt");
            pluginVersion.HelpFileRu = fileName;
        }
        
        if (updatePluginVersionDto.HelpFileKz != null)
        {
            if (pluginVersion.HelpFileKz != null)
            {
                var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "upload", pluginVersion.HelpFileKz);
                
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            
            var fileName = await SaveFile(updatePluginVersionDto.HelpFileKz, "txt");
            pluginVersion.HelpFileKz = fileName;
        }

        _dbContext.PluginVersions.Update(pluginVersion);
        await _dbContext.SaveChangesAsync();

        return Ok(_mapper.Map<PluginVersionDto>(pluginVersion));
    }
    
    [HttpPost, HttpDelete]
    [Route("{pluginVersionId}/beta")]
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
    [Route("{pluginVersionId}/deprecated")]
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