using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PluginStore.Api.Data;
using PluginStore.Api.Models.Dto;
using PluginStore.Api.Services.Interfaces;

namespace PluginStore.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserController(ApplicationDbContext dbContext, IMapper mapper, IUserService userService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = _mapper.Map<List<UserDto>>(await _dbContext.Users.ToListAsync());

        return Ok(users);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetOne(Guid userId)
    {
        var user = _mapper.Map<UserDto>(await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId));

        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
    {
        var candidate = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == createUserDto.Email);

        if (candidate != null)
        {
            return BadRequest("Пользователь с таким email уже существует");
        }

        var user = await _userService.CreateUser(createUserDto);
        
        var userDto = _mapper.Map<UserDto>(user);
        
        return Ok(userDto);
    }

    private static string GeneratePassword(int length = 6)
    {
        var random = new Random();
        var result = "";

        while (result.Length < length)
        {
            var n = random.Next();
            if ((47 < n && n < 58) || (64 < n && n < 91) || (96 < n && n < 123))
            {
                result += Convert.ToChar(n);
            }
        }

        return result;
    }
}