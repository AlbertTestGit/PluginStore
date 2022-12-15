using Microsoft.EntityFrameworkCore;

namespace PluginStore.Api.Models;

public static class Roles
{
    public const string SubsoilUser = "Недропользователь";
    public const string DesignInstitute = "Проектный институт";
    public const string ProjectAuthor = "Автор проекта";
    public const string ExpertGeologist = "Эксперт геолог";
    public const string ExpertDeveloper = "Эксперт разработчик";
    public const string Operator = "Оператор";
    public const string Administrator = "Администратор";
}

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string Role { get; set; } = Roles.SubsoilUser;
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}