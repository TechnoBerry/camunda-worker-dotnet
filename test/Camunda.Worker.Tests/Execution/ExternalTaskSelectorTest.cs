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
        private readonly Mock<IExternalTaskClient> _clientMock = new();
        private readonly Mock<ITopicsProvider> _topicsProviderMock = new();
        private readonly CamundaWorkerOptions _workerOptions = new("testWorker");

        private readonly IOptions<SelectorOptions> _selectorOptions = Options.Create(new SelectorOptions
        {
            AsyncResponseTimeout = 5_000
        });

        private readonly ExternalTaskSelector _selector;

        public ExternalTaskSelectorTest()
        {
            _topicsProviderMock.Setup(provider => provider.GetTopics())
                .Returns(Array.Empty<FetchAndLockRequest.Topic>());

            _selector = new ExternalTaskSelector(
                _clientMock.Object,
                _topicsProviderMock.Object,
                _workerOptions,
                _selectorOptions
            );
        }

        [Fact]
        public async Task TestSuccessfullySelection()
        {
            _clientMock
                .Setup(client =>
                    client.FetchAndLockAsync(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExternalTask>());

            var result = await _selector.SelectAsync();

            Assert.Empty(result);
            _clientMock.VerifyAll();
        }

        [Fact]
        public async Task TestCancelledSelection()
        {
            var cts = new CancellationTokenSource();
            var tcs = new TaskCompletionSource<List<ExternalTask>>();

            await using var reg = cts.Token.Register(() => tcs.SetCanceled());

            _clientMock
                .Setup(client =>
                    client.FetchAndLockAsync(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()))
                .Returns(tcs.Task);

            var resultTask = _selector.SelectAsync(cts.Token);

            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(() => resultTask);
            _clientMock.VerifyAll();
        }
    }
}
