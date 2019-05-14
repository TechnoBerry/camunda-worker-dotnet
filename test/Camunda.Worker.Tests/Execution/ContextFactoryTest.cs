using System;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class ContextFactoryTest
    {
        private readonly Mock<IServiceProvider> _providerMock = new Mock<IServiceProvider>();
        private readonly Mock<IServiceScope> _scopeMock = new Mock<IServiceScope>();
        private readonly IContextFactory _factory;

        public ContextFactoryTest()
        {
            _scopeMock.SetupGet(scope => scope.ServiceProvider).Returns(_providerMock.Object);

            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            scopeFactoryMock.Setup(factory => factory.CreateScope())
                .Returns(_scopeMock.Object);

            _providerMock.Setup(provider => provider.GetService(typeof(IServiceScopeFactory)))
                .Returns(scopeFactoryMock.Object);

            var clientMock = new Mock<IExternalTaskClient>();
            _providerMock.Setup(provider => provider.GetService(typeof(IExternalTaskClient)))
                .Returns(clientMock.Object);

            _factory = new ContextFactory(_providerMock.Object);
        }

        [Fact]
        public void TestMakeContext()
        {
            var task = new ExternalTask("id", "worker", "topic");

            var result = _factory.MakeContext(task);

            Assert.Same(task, result.Task);
            Assert.Same(_providerMock.Object, result.ServiceProvider);
        }
    }
}
