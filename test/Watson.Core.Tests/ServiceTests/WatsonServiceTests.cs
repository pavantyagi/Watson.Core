using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Watson.Core.Tests.FakeResponses;
using Watson.Core.Tests.Fakes;
using Xunit;

// ReSharper disable ExceptionNotDocumented

namespace Watson.Core.Tests.ServiceTests
{
    public class WatsonServiceTests
    {
        [Fact]
        public void Username_SetNullByConstructor_ThrowArgumentNullException()
        {
            var exception =
                Record.Exception(() => new FakeWatsonService(null, "password", new HttpClient(), new WatsonSettings()));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void Password_SetNullByConstructor_ThrowArgumentNullException()
        {
            var exception =
                Record.Exception(() => new FakeWatsonService("username", null, new HttpClient(), new WatsonSettings()));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void HttpClient_SetNullByConstructor_ThrowArgumentNullException()
        {
            var exception =
                Record.Exception(() => new FakeWatsonService("username", "password", null, new WatsonSettings()));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void WatsonSettings_SetNullByConstructor_ThrowArgumentNullException()
        {
            var exception = Record.Exception(() => new FakeWatsonService("username", "password", new HttpClient(), null));
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void HttpClient_SetByConstructor_NotNull()
        {
            var service = new FakeWatsonService("username", "password", new HttpClient(), new WatsonSettings());
            Assert.NotNull(service);
            Assert.NotNull(service.HttpClient);
        }

        [Fact]
        public void WatsonSettings_SetByConstructor_NotNull()
        {
            var service = new FakeWatsonService("username", "password", new HttpClient(), new WatsonSettings());
            Assert.NotNull(service);
            Assert.NotNull(service.Settings);
        }

        [Fact]
        public void WatsonSettings_SetByConstructor_Equal()
        {
            var client = new HttpClient();
            var settings = new WatsonSettings
            {
                LearningOptOut = false
            };
            var service = new FakeWatsonService("username", "password", client, settings);
            Assert.Equal(service.Settings.LearningOptOut, false);

            settings = new WatsonSettings
            {
                LearningOptOut = true
            };
            service = new FakeWatsonService("username", "password", client, settings);
            Assert.Equal(service.Settings.LearningOptOut, true);
        }

        [Fact]
        public void HttpClient_SetByConstructor_Equal()
        {
            var client = new HttpClient {BaseAddress = new Uri("https://www.google.com")};
            var service = new FakeWatsonService("username", "password", client, new WatsonSettings());
            Assert.Equal(client.BaseAddress, service.HttpClient.BaseAddress);
        }

        [Fact]
        public void ApiKey_SetByConstructor_Equal()
        {
            var service = new FakeWatsonService("ausername", "apassword", new HttpClient(), new WatsonSettings());
            Assert.Equal("ausername:apassword", service.ApiKey);
        }

        [Fact]
        public void HttpClient_SetByConstructor_IsConfigured()
        {
            var service = new FakeWatsonService("ausername", "apassword", new HttpClient(), new WatsonSettings());

            var encoding = Encoding.UTF8;
            var byteArray = encoding.GetBytes(service.ApiKey);
            var apiKeyBase64 = Convert.ToBase64String(byteArray);
            var actualAuthorization = new AuthenticationHeaderValue("Basic", apiKeyBase64);

            //Assert that Authorization is set
            var expectedAuthorization = service.HttpClient.DefaultRequestHeaders.Authorization;
            Assert.Equal(expectedAuthorization.Parameter, actualAuthorization.Parameter);
        }

        [Fact]
        public async Task SendRequestAsync_WithNullMessage_ThrowsArgumentNullException()
        {
            var service = new FakeWatsonService("ausername", "apassword", new HttpClient(), new WatsonSettings());

            var exception =
                await
                    Record.ExceptionAsync(async () => await service.SendRequestAsync<bool>(null).ConfigureAwait(false))
                        .ConfigureAwait(false);
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("http://example.org/test", "http://example.org/test", "true")]
        public async Task SendRequestAsyncBool(string requestUrl, string contentUrl, string contentAtUrl)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, contentUrl);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(contentAtUrl)};
            var httpClient = new HttpClient(new FakeHttpMessageHandler(contentUrl, responseMessage));
            var service = new FakeWatsonService("ausername", "apassword", httpClient, new WatsonSettings());

            var response = await service.SendRequestAsync<bool>(requestMessage).ConfigureAwait(false);

            Assert.Equal(response, Convert.ToBoolean(contentAtUrl));
        }

        [Fact]
        public async Task SendRequestAsync_ThrowWatsonException()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://example.org/request");
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent("true")};
            var httpClient = new HttpClient(new FakeHttpMessageHandler("http://example.org/404", responseMessage));
            var service = new FakeWatsonService("ausername", "apassword", httpClient, new WatsonSettings());

            var exception =
                await
                    Record.ExceptionAsync(
                        async () => await service.SendRequestAsync<bool>(requestMessage).ConfigureAwait(false))
                        .ConfigureAwait(false);
            Assert.NotNull(exception);
            Assert.IsType<WatsonException>(exception);
        }

        [Fact]
        public async Task SendRequestAsync_With401ErrorMessage_ThrowWatsonException()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://example.org/request");
            var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(FakeErrors.Fake401Error)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler("http://example.org/request", responseMessage));
            var service = new FakeWatsonService("ausername", "apassword", httpClient, new WatsonSettings());

            var exception =
                await
                    Record.ExceptionAsync(
                        async () => await service.SendRequestAsync<bool>(requestMessage).ConfigureAwait(false))
                        .ConfigureAwait(false);
            Assert.NotNull(exception);
            Assert.IsType<WatsonException>(exception);
        }

        [Fact]
        public async Task SendRequestWithRawResponseAsync_WithNullMessage_ThrowsArgumentNullException()
        {
            var service = new FakeWatsonService("ausername", "apassword", new HttpClient(), new WatsonSettings());

            var exception =
                await
                    Record.ExceptionAsync(
                        async () => await service.SendRequestWithRawResponseAsync(null).ConfigureAwait(false))
                        .ConfigureAwait(false);
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("http://example.org/test", "http://example.org/test", "Hello World")]
        public async Task SendRequestWithRawResponseAsync(string requestUrl, string contentUrl, string contentAtUrl)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, contentUrl);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(contentAtUrl)};
            var httpClient = new HttpClient(new FakeHttpMessageHandler(contentUrl, responseMessage));
            var service = new FakeWatsonService("ausername", "apassword", httpClient, new WatsonSettings());

            var stringResponse = await service.SendRequestWithRawResponseAsync(requestMessage).ConfigureAwait(false);

            Assert.Equal(stringResponse, contentAtUrl);
        }

        [Fact]
        public async Task SendRequestWithRawResponseAsync_With401ErrorMessage_ThrowWatsonException()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://example.org/request");
            var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(FakeErrors.Fake401Error)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler("http://example.org/request", responseMessage));
            var service = new FakeWatsonService("ausername", "apassword", httpClient, new WatsonSettings());

            var exception =
                await
                    Record.ExceptionAsync(
                        async () => await service.SendRequestWithRawResponseAsync(requestMessage).ConfigureAwait(false))
                        .ConfigureAwait(false);
            Assert.NotNull(exception);
            Assert.IsType<WatsonException>(exception);
        }
    }
}