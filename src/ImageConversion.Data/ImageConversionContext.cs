using ImageConversion.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace ImageConversion.Data;

public class ImageConversionContext : DbContext
{
    public ImageConversionContext(DbContextOptions<ImageConversionContext> options) : base(options)
    {

    }

    public DbSet<ImageFile> ImageFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImageFile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.SourceExtension).IsRequired();
            entity.Property(e => e.TargetExtension).IsRequired();
            entity.Property(e => e.ProcessStatus).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
