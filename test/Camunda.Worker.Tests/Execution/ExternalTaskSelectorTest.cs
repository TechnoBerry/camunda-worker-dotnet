using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskSelectorTest
    {
        private readonly Mock<IExternalTaskClient> _clientMock = new Mock<IExternalTaskClient>();

        private readonly IOptions<CamundaWorkerOptions> _options = Options.Create(new CamundaWorkerOptions
        {
            WorkerId = "testWorker",
            BaseUri = new Uri("http://test"),
            AsyncResponseTimeout = 5_000
        });

        private readonly ExternalTaskSelector _selector;

        public ExternalTaskSelectorTest()
        {
            var provider = new ServiceCollection().AddSingleton(_clientMock.Object).BuildServiceProvider();
            _selector = new ExternalTaskSelector(
                provider,
                _options
            );
        }

        [Fact]
        public async Task TestSuccessfullySelection()
        {
            _clientMock
                .Setup(client =>
                    client.FetchAndLockAsync(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExternalTask>());

            var result = await _selector.SelectAsync(new FetchAndLockRequest.Topic[0]);

            Assert.Empty(result);
            _clientMock.VerifyAll();
        }
    }
}
