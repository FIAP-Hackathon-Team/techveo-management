using System.Net.Http.Json;
using TechVeo.Management.Application.Dto;
using TechVeo.Management.Application.Services.Interfaces;

namespace TechVeo.Management.Infra.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDto> GetUserBydIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _httpClient.GetFromJsonAsync<UserDto>($"/v1/users/{userId}", cancellationToken);
            if (user == null)
            {
                throw new Exception("Failed to retrieve user information from the authentication service.");
            }

            return user;
        }
    }
}
