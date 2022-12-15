using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using PluginStore.Api.Data;
using PluginStore.Api.Models;
using PluginStore.Api.Models.Dto;
using PluginStore.Api.Services.Interfaces;

namespace PluginStore.Api.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<User> CreateUser(CreateUserDto userDto)
    {
        var password = userDto.Password ?? GeneratePassword();

        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        var user = _mapper.Map<User>(userDto);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = salt;

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        //_emailService.SendPasswordToEmail(user.Email, user.Name, password);
        Console.WriteLine(password);
        
        return user;
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