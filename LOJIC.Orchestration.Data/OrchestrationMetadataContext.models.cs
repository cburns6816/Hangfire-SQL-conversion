using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LOJIC.Orchestration.Data.Models.Heartbeat;

namespace LOJIC.Orchestration.Data
{
    public partial class OrchestrationMetadataContext
    {
        public IQueryable<ARCGISServerSqlHeartbeat> ArcgisServerSqlHeartbeats =>
            Set<ARCGISServerSqlHeartbeat>()
                .OrderByDescending(heartbeat => heartbeat.Created);
        public IQueryable<ARCGISEnterpriseSqlHeartbeat> ArcgisEnterpriseSqlHeartbeats =>
            Set<ARCGISEnterpriseSqlHeartbeat>()
                .OrderByDescending(heartbeat => heartbeat.Created);
    }
}
