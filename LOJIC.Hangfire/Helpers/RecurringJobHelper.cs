using System;
using System.Linq.Expressions;
using Hangfire;
using LOJIC.Hangfire.Jobs;

namespace LOJIC.Hangfire.Helpers
{
    public static class RecurringJobHelper
    {
        public static void AddOrUpdate<T>(Expression<Action<T>> methodCall, Func<string> cronExpression)
            where T : BaseJob
        {
            RecurringJob.AddOrUpdate(typeof(T).Name, methodCall, cronExpression);
        }
        public static void Trigger<T>()
            where T : BaseJob
        {
            RecurringJob.Trigger(typeof(T).Name);
        }
    }
}
