namespace TechVeo.Management.Application.Dto
{
    public record UserDto(Guid Id, string Name, string Username, string? Email, string Role);
}
