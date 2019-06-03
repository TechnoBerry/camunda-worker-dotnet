using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class PipelineBuilderTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();
        private readonly Mock<IExternalTaskRouter> _routerMock = new Mock<IExternalTaskRouter>();
        private readonly IServiceCollection _services = new ServiceCollection();

        public PipelineBuilderTest()
        {
            _services.AddTransient(_ => _routerMock.Object);
            _contextMock.SetupGet(ctx => ctx.ServiceProvider)
                .Returns(_services.BuildServiceProvider());
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        public async Task TestBuildPipeline(int calls)
        {
            IPipelineBuilder builder = new PipelineBuilder(_services);

            var numsIn = new List<int>(calls);
            var numsOut = new List<int>(calls);

            Enumerable.Range(0, calls)
                .Select(i => (Func<ExternalTaskDelegate, ExternalTaskDelegate>) (next => async ctx =>
                {
                    numsIn.Add(i);

                    await next(ctx);

                    numsOut.Add(i);
                }))
                .Aggregate(builder, (b, func) => b.Use(func));

            var result = builder.Build();
            await result(_contextMock.Object);

            _routerMock.Verify(router => router.RouteAsync(It.IsAny<IExternalTaskContext>()), Times.Once());
            Assert.Equal(calls, numsIn.Count);
            Assert.Equal(calls, numsOut.Count);

            Assert.Equal(numsIn.Count, numsIn.Distinct().Count());
            Assert.Equal(numsOut.Count, numsOut.Distinct().Count());

            Assert.Equal(numsIn, ((IEnumerable<int>) numsOut).Reverse().ToList());
        }
    }
}
