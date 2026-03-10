using TechVeo.Management.Application.Dto;

namespace TechVeo.Management.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<UserDto> GetUserBydIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
