using System.Threading.Tasks;

namespace Camunda.Worker;

public delegate Task ExternalTaskDelegate(IExternalTaskContext context);
