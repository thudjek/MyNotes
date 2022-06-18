using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;
public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasMany(n => n.Permissions)
            .WithOne(p => p.Note)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Property(n => n.Title)
            .HasMaxLength(100);

        builder.Property(n => n.Content)
            .HasMaxLength(1000);
    }
}