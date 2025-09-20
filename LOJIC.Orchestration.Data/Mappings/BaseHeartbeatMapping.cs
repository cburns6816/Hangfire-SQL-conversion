using LOJIC.Orchestration.Data.Models;
using LOJIC.Orchestration.Data.Models.Heartbeat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LOJIC.Orchestration.Data.Mappings
{
    class BaseHeartbeatMapping : IEntityTypeConfiguration<BaseServiceHeartbeat>
    {
        public void Configure(EntityTypeBuilder<BaseServiceHeartbeat> builder)
        {
            builder.ToTable("ServiceHeartbeat")
                .HasDiscriminator<int>("ServiceType")
                .HasValue<ARCGISServerSqlHeartbeat>(1)
                .HasValue<ARCGISEnterpriseSqlHeartbeat>(2);
        }
    }
}
