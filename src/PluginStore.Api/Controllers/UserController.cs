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

        return CreatedAtAction(nameof(GetOne), new { userId = user.UserId }, userDto);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto updateUserDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == updateUserDto.UserId);

        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        var updatedUser = await _userService.UpdateUser(user, updateUserDto);

        return Ok(_mapper.Map<UserDto>(updatedUser));
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(Guid userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        await _userService.DeleteUser(user);

        return NoContent();
    }
}