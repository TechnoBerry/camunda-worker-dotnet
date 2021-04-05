using System;
using Bogus;
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
            // Arrange
            var externalTask = new Faker<ExternalTask>()
                .CustomInstantiator(faker => new ExternalTask(
                    faker.Random.Guid().ToString(),
                    faker.Random.Word(),
                    faker.Random.Word())
                )
                .Generate();
            var serviceProviderMock = new Mock<IServiceProvider>();

            // Act
            var result = _factory.Create(externalTask, serviceProviderMock.Object);

            // Assert
            Assert.Same(externalTask, result.Task);
            Assert.StrictEqual(serviceProviderMock.Object, result.ServiceProvider);
        }
    }
}
