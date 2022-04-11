using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker;

public class PipelineBuilderTest
{
    private readonly Mock<IExternalTaskContext> _contextMock = new();
    private readonly IServiceProvider _serviceProvider;

    public PipelineBuilderTest()
    {
        var services = new ServiceCollection();
        _serviceProvider = services.BuildServiceProvider();
        _contextMock.SetupGet(ctx => ctx.ServiceProvider)
            .Returns(_serviceProvider);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public async Task TestBuildPipeline(int calls)
    {
        IPipelineBuilder builder = new PipelineBuilder(new Faker().Random.String(), _serviceProvider);

        Task LastDelegate(IExternalTaskContext context)
        {
            return Task.CompletedTask;
        }

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

        var result = builder.Build(LastDelegate);
        await result(_contextMock.Object);

        Assert.Equal(calls, numsIn.Count);
        Assert.Equal(calls, numsOut.Count);

        Assert.Equal(numsIn.Count, numsIn.Distinct().Count());
        Assert.Equal(numsOut.Count, numsOut.Distinct().Count());

        Assert.Equal(numsIn, ((IEnumerable<int>) numsOut).Reverse().ToList());
    }
}
