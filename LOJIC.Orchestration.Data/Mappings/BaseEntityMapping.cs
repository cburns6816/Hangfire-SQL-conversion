using LOJIC.Orchestration.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LOJIC.Orchestration.Data.Mappings
{
    public abstract class BaseEntityMapping<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(entity => entity.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(entity => entity.Created)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();
        }
    }
}
