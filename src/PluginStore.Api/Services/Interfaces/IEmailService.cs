using PluginStore.Api.Models.Dto;

namespace PluginStore.Api.Services.Interfaces;

public interface IEmailService
{
    void SendPasswordToEmail(string to, string name, string password);
}