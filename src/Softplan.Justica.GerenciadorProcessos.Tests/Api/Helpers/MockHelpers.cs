using Microsoft.AspNetCore.Http;
using Moq;

namespace Softplan.Justica.GerenciadorProcessos.Tests.Api.Helpers
{
    public static class MockHelpers
    {
        public static HttpContext CreateHttpContextMock()
        {
            var headerDictionary = new HeaderDictionary();

            var response = new Mock<HttpResponse>();
            response.SetupGet(r => r.Headers).Returns(headerDictionary);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(a => a.Response).Returns(response.Object);

            return httpContext.Object;
        }
    }
}