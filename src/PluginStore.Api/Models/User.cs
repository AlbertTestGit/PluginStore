using System.ComponentModel.DataAnnotations;
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
    public const string Developer = "Разработчик";
}

public class RoleAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var role = value as string;

        return role switch
        {
            Roles.SubsoilUser => ValidationResult.Success,
            Roles.DesignInstitute => ValidationResult.Success,
            Roles.ProjectAuthor => ValidationResult.Success,
            Roles.ExpertGeologist => ValidationResult.Success,
            Roles.ExpertDeveloper => ValidationResult.Success,
            Roles.Operator => ValidationResult.Success,
            Roles.Administrator => ValidationResult.Success,
            Roles.Developer => ValidationResult.Success,
            _ => new ValidationResult("Недопустимая роль")
        };
    }
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