using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class NoneResultTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new();

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            var result = new NoneResult();
            await result.ExecuteResultAsync(_contextMock.Object);
            _contextMock.VerifyNoOtherCalls();
        }
    }
}
