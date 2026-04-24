using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentia.Domain.Entities;

namespace Sentia.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToView("AspNetUsers");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.UserName);
    }
}