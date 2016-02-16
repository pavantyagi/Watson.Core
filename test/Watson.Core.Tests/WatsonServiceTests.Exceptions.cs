using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Watson.Core.Tests.Mocks;
using Xunit;

// ReSharper disable ExceptionNotDocumented

namespace Watson.Core.Tests
{
    public partial class WatsonServiceTests
    {
        public static IEnumerable<object[]> ConstructorData => new[]
        {
            new object[] {null, null, null, null},
            new object[] {null, "password", new HttpClient(), new WatsonSettings()},
            new object[] {"username", null, new HttpClient(), new WatsonSettings()},
            new object[] {"username", "password", null, new WatsonSettings()},
            new object[] {"username", "password", new HttpClient(), null}
        };

        [Theory, MemberData("ConstructorData")]
        public void Constructor_WithNullParameters_ThrowsArgumentNullException(string username, string password,
            HttpClient httpClient, WatsonSettings settings)
        {
            var exception =
                Record.Exception(() => new MockWatsonService(username, password, httpClient, settings));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task SendRequestAsync_WithNullMessage_ThrowsArgumentNullException()
        {
            var service = new MockWatsonService("ausername", "apassword", new HttpClient(), new WatsonSettings());

            var exception =
                await
                    Record.ExceptionAsync(
                        async () => await service.SendRequestAsync(null).ConfigureAwait(false))
                        .ConfigureAwait(false);
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task SendRequestGenericAsync_WithNullMessage_ThrowsArgumentNullException()
        {
            var service = new MockWatsonService("ausername", "apassword", new HttpClient(), new WatsonSettings());

            var exception =
                await
                    Record.ExceptionAsync(async () => await service.SendRequestAsync<bool>(null).ConfigureAwait(false))
                        .ConfigureAwait(false);
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("http://example.org/request", "http://example.org/404", "", 404, "404 Not Found")]
        [InlineData("http://example.org/request", "http://example.org/request", MockErrors.Error401RawHtml, 401,
            "401 Unauthorized")]
        [InlineData("http://example.org/request", "http://example.org/request", MockErrors.Error400, 400,
            "The number of words 5 is less than the minimum number of words required for analysis: 100")]
        [InlineData("http://example.org/request", "http://example.org/request", MockErrors.Error415, 415,
            "The Media Type [text/plain] of the input document is not supported. Auto correction was attempted, but the auto detected media type [application/x-tika-ooxml] is also not supported. Supported Media Types are: application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/pdf, text/html, application/xhtml+xml ."
            )]
        public async Task SendRequestAsync_ThrowWatsonException(string requestUrl, string handlerUrl,
            string responseContent, int statusCode, string exceptionMessage)
        {
            var httpStatusCode = (HttpStatusCode) Enum.ToObject(typeof (HttpStatusCode), statusCode);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var responseMessage = new HttpResponseMessage(httpStatusCode) {Content = new StringContent(responseContent)};
            var httpClient = new HttpClient(new MockHttpMessageHandler(handlerUrl, responseMessage));
            var service = new MockWatsonService("ausername", "apassword", httpClient, new WatsonSettings());

            var exception =
                await
                    Record.ExceptionAsync(
                        async () => await service.SendRequestAsync(requestMessage).ConfigureAwait(false))
                        .ConfigureAwait(false);
            Assert.NotNull(exception);
            Assert.IsType<WatsonException>(exception);

            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData("http://example.org/request", "http://example.org/404", "", 404, "404 Not Found")]
        [InlineData("http://example.org/request", "http://example.org/request", MockErrors.Error401RawHtml, 401,
            "401 Unauthorized")]
        [InlineData("http://example.org/request", "http://example.org/request", MockErrors.Error400, 400,
            "The number of words 5 is less than the minimum number of words required for analysis: 100")]
        [InlineData("http://example.org/request", "http://example.org/request", MockErrors.Error415, 415,
            "The Media Type [text/plain] of the input document is not supported. Auto correction was attempted, but the auto detected media type [application/x-tika-ooxml] is also not supported. Supported Media Types are: application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/pdf, text/html, application/xhtml+xml ."
            )]
        public async Task SendRequestGenericAsync_ThrowWatsonException(string requestUrl, string handlerUrl,
            string responseContent, int statusCode, string exceptionMessage)
        {
            var httpStatusCode = (HttpStatusCode) Enum.ToObject(typeof (HttpStatusCode), statusCode);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var responseMessage = new HttpResponseMessage(httpStatusCode) {Content = new StringContent(responseContent)};
            var httpClient = new HttpClient(new MockHttpMessageHandler(handlerUrl, responseMessage));
            var service = new MockWatsonService("ausername", "apassword", httpClient, new WatsonSettings());

            var exception =
                await
                    Record.ExceptionAsync(
                        async () => await service.SendRequestAsync<bool>(requestMessage).ConfigureAwait(false))
                        .ConfigureAwait(false);
            Assert.NotNull(exception);
            Assert.IsType<WatsonException>(exception);

            Assert.Equal(exceptionMessage, exception.Message);
        }
    }
}