using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Camunda.Worker.Extensions
{
    internal static class ExternalTaskContextExtensions
    {
        internal static void LogWarning<T>(this IExternalTaskContext context, string message, params object[] args)
        {
            var logger = context.ServiceProvider.GetService<ILogger<T>>();
            logger?.LogWarning(message, args);
        }
    }
}
