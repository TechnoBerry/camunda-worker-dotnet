using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
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
            _selector = new ExternalTaskSelector(
                _clientMock.Object,
                _options
            );
        }

        [Fact]
        public async Task TestSuccessfullySelection()
        {
            _clientMock
                .Setup(client => client.FetchAndLock(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExternalTask>());

            var result = await _selector.SelectAsync(new FetchAndLockRequest.Topic[0]);

            Assert.Empty(result);
            _clientMock.VerifyAll();
        }
    }
}
