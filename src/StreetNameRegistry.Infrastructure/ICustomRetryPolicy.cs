using System;
using System.Threading.Tasks;

namespace StreetNameRegistry.Infrastructure
{
    public interface ICustomRetryPolicy
    {
        Task Retry(Func<Task> functionToRetry);
    }
}
