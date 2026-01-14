using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechVeo.Api.Domain.Entities;

namespace TechVeo.Api.Infra.Persistence.Mappings;

public class VideoMap : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.ToTable("Video");
    }
}
