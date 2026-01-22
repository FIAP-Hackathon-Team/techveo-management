using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechVeo.Management.Domain.Entities;
using TechVeo.Management.Domain.Repositories;
using TechVeo.Management.Infra.Persistence.Contexts;

namespace TechVeo.Management.Infra.Persistence.Repositories;

internal class VideoRepository(VideoContext dbContext) : IVideoRepository
{
    private readonly DbSet<Video> _videos = dbContext.Videos;

    public async Task<Guid> AddAsync(Video Video)
    {
        throw new Exception();
    }

    public Task<Video?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Video?> GetByUserIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
