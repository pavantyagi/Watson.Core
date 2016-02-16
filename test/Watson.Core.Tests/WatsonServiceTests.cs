using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Watson.Core.Tests.Mocks;
using Xunit;

// ReSharper disable ExceptionNotDocumented

namespace Watson.Core.Tests
{
    public partial class WatsonServiceTests
    {
        [Fact]
        public void HttpClient_SetByConstructor_NotNull()
        {
            var service = new MockWatsonService("username", "password", new HttpClient(), new WatsonSettings());
            Assert.NotNull(service);
            Assert.NotNull(service.HttpClient);
        }

        [Fact]
        public void WatsonSettings_SetByConstructor_NotNull()
        {
            var service = new MockWatsonService("username", "password", new HttpClient(), new WatsonSettings());
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
            var service = new MockWatsonService("username", "password", client, settings);
            Assert.Equal(service.Settings.LearningOptOut, false);

            settings = new WatsonSettings
            {
                LearningOptOut = true
            };
            service = new MockWatsonService("username", "password", client, settings);
            Assert.Equal(service.Settings.LearningOptOut, true);
        }

        [Fact]
        public void HttpClient_SetByConstructor_Equal()
        {
            var client = new HttpClient {BaseAddress = new Uri("https://www.google.com")};
            var service = new MockWatsonService("username", "password", client, new WatsonSettings());
            Assert.Equal(client.BaseAddress, service.HttpClient.BaseAddress);
        }

        [Fact]
        public void ApiKey_SetByConstructor_Equal()
        {
            var service = new MockWatsonService("ausername", "apassword", new HttpClient(), new WatsonSettings());
            Assert.Equal("ausername:apassword", service.ApiKey);
        }

        [Fact]
        public void HttpClient_SetByConstructor_IsConfigured()
        {
            var service = new MockWatsonService("ausername", "apassword", new HttpClient(), new WatsonSettings());

            var encoding = Encoding.UTF8;
            var byteArray = encoding.GetBytes(service.ApiKey);
            var apiKeyBase64 = Convert.ToBase64String(byteArray);
            var actualAuthorization = new AuthenticationHeaderValue("Basic", apiKeyBase64);

            //Assert that Authorization is set
            var expectedAuthorization = service.HttpClient.DefaultRequestHeaders.Authorization;
            Assert.Equal(expectedAuthorization.Parameter, actualAuthorization.Parameter);
        }

        [Theory]
        [InlineData(22.05)]
        [InlineData(99)]
        public async Task SendRequestAsync<T>(T responseContent)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://example.org/url");
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent.ToString())
            };
            var httpClient = new HttpClient(new MockHttpMessageHandler("http://example.org/url", responseMessage));
            var service = new MockWatsonService("ausername", "apassword", httpClient, new WatsonSettings());

            var response = await service.SendRequestAsync<T>(requestMessage).ConfigureAwait(false);

            Assert.Equal(response, responseContent);
        }

        [Theory]
        [InlineData("http://example.org/url", "Hello World")]
        public async Task SendRequestAsync(string requestUrl, string responseContent)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };
            var httpClient = new HttpClient(new MockHttpMessageHandler(requestUrl, responseMessage));
            var service = new MockWatsonService("ausername", "apassword", httpClient, new WatsonSettings());

            var stringResponse = await service.SendRequestAsync(requestMessage).ConfigureAwait(false);

            Assert.Equal(stringResponse, responseContent);
        }
    }
}