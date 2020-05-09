using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class ContextFactoryTest
    {
        private readonly IContextFactory _factory;

        public ContextFactoryTest()
        {
            IServiceProvider provider = new ServiceCollection()
                .BuildServiceProvider();

            _factory = new ContextFactory(provider);
        }

        [Fact]
        public void TestMakeContext()
        {
            var task = new ExternalTask("id", "worker", "topic");

            var result = _factory.MakeContext(task);

            Assert.Same(task, result.Task);
            Assert.NotNull(result.ServiceProvider);
        }
    }
}
