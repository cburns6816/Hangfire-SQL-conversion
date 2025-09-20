using Microsoft.Extensions.Logging;

namespace LOJIC.Hangfire.Jobs
{
    public abstract class BaseJob
    {
        protected readonly ILogger Logger;
    }
}
