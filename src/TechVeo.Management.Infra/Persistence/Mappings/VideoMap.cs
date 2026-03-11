using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TechVeo.Management.Infra.Persistence.Mappings;

public class VideoMap : IEntityTypeConfiguration<Domain.Entities.Video>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Video> builder)
    {
        builder.ToTable("Management");
    }
}
