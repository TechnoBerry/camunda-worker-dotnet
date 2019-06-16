using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskRouterTest
    {
        private readonly Mock<IServiceProvider> _providerMock = new Mock<IServiceProvider>();
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();

        private readonly Mock<IHandlerDelegateProvider>
            _handlerFactoryProviderMock = new Mock<IHandlerDelegateProvider>();

        public ExternalTaskRouterTest()
        {
            _contextMock.SetupGet(context => context.ServiceProvider).Returns(_providerMock.Object);
            _contextMock.SetupGet(context => context.Task).Returns(new ExternalTask("1", "testWorker", "testTopic"));
        }

        [Fact]
        public async Task TestRouteAsync()
        {
            var handlerMock = MakeHandlerMock();

            handlerMock.Setup(handler => handler.HandleAsync(It.IsAny<IExternalTaskContext>()))
                .Returns(Task.CompletedTask);

            var executor = MakeExecutor();

            await executor.RouteAsync(_contextMock.Object);

            handlerMock.Verify(handler => handler.HandleAsync(It.IsAny<IExternalTaskContext>()), Times.Once());
        }

        private Mock<IExternalTaskHandler> MakeHandlerMock()
        {
            var handlerMock = new Mock<IExternalTaskHandler>();
            _handlerFactoryProviderMock.Setup(factory => factory.GetHandlerFactory(It.IsAny<ExternalTask>()))
                .Returns(provider => handlerMock.Object);
            return handlerMock;
        }

        [Fact]
        public async Task TestRouteAsyncWithNullArg()
        {
            var executor = MakeExecutor();

            await Assert.ThrowsAsync<ArgumentNullException>(() => executor.RouteAsync(null));
        }

        private IExternalTaskRouter MakeExecutor()
        {
            return new ExternalTaskRouter(
                _handlerFactoryProviderMock.Object
            );
        }
    }
}
