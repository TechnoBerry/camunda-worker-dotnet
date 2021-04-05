using System;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class ContextFactoryTest
    {
        private readonly IContextFactory _factory;

        public ContextFactoryTest()
        {
            _factory = new ContextFactory();
        }

        [Fact]
        public void TestCreate()
        {
            var task = new ExternalTask("id", "worker", "topic");

            var result = _factory.Create(task, new Mock<IServiceProvider>().Object);

            Assert.Same(task, result.Task);
            Assert.NotNull(result.ServiceProvider);
        }
    }
}
