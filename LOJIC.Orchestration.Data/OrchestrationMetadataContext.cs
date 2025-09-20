using System.Collections.Generic;
using System.Linq;
using LOJIC.Orchestration.Data.Extensions;
using LOJIC.Orchestration.Data.Models;
using LOJIC.Orchestration.Data.Models.Heartbeat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LOJIC.Orchestration.Data
{
    public partial class OrchestrationMetadataContext : DbContext
    {
        public OrchestrationMetadataContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyByNameSpace("LOJIC.Orchestration.Data.Mappings");
            base.OnModelCreating(modelBuilder);
        }

        public static void Seed<T>(EntityTypeBuilder<T> builder, IEnumerable<T> entities) where T : BaseEntity
        {
        }

    

    }
}
