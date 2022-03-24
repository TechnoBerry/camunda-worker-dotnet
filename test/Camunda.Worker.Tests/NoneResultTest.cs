using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker;

public class NoneResultTest
{
    private readonly Mock<IExternalTaskContext> _contextMock = new();

    [Fact]
    public async Task TestExecuteResultAsync()
    {
        // Arrange
        var result = new NoneResult();

        // Act
        await result.ExecuteResultAsync(_contextMock.Object);

        // Assert
        _contextMock.VerifyNoOtherCalls();
    }
}
