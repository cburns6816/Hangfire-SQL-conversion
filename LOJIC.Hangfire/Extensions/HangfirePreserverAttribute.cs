using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;

namespace LOJIC.Hangfire.Extensions
{
    public class HangfirePreserverAttribute : JobFilterAttribute, IApplyStateFilter
    {
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            // Only activate this filter when we're enqueueing
            if (!(context.NewState is EnqueuedState enqueuedState)) return;

            var originalQueue = SerializationHelper.Deserialize<string>(
                context.Connection.GetJobParameter(
                    context.BackgroundJob.Id,
                    "OriginalQueue"));

            if (originalQueue != null)
            {
                enqueuedState.Queue = originalQueue;
            }
            else
            {
                context.Connection.SetJobParameter(
                    context.BackgroundJob.Id,
                    "OriginalQueue",
                    SerializationHelper.Serialize(enqueuedState.Queue));
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
        }
    }
}
