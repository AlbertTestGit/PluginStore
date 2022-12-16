using PluginStore.Api.Models;
using PluginStore.Api.Models.Dto;

namespace PluginStore.Api.Services.Interfaces;

public interface IUserService
{
    Task<User> CreateUser(CreateUserDto userDto);
    Task<User> UpdateUser(User user, UpdateUserDto userDto);
    Task DeleteUser(User user);
}