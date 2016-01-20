using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Watson.Core.Tests.FakeResponses;
using Watson.Core.Tests.Fakes;

// ReSharper disable ExceptionNotDocumented

namespace Watson.Core.Tests.ServiceTests
{
    [TestClass]
    public class WatsonServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Username_SetNullByConstructor_ThrowArgumentNullException()
        {
            var service = new FakeService(null, "password", new HttpClient(), new WatsonSettings());
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Password_SetNullByConstructor_ThrowArgumentNullException()
        {
            var service = new FakeService("username", null, new HttpClient(), new WatsonSettings());
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HttpClient_SetNullByConstructor_ThrowArgumentNullException()
        {
            var service = new FakeService("username", "password", null, new WatsonSettings());
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WatsonSettings_SetNullByConstructor_ThrowArgumentNullException()
        {
            var service = new FakeService("username", "password", new HttpClient(), null);
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void HttpClient_SetByConstructor_IsNotNull()
        {
            var service = new FakeService("username", "password", new HttpClient(), new WatsonSettings());
            Assert.IsNotNull(service);
            Assert.IsNotNull(service.HttpClient);
        }

        [TestMethod]
        public void WatsonSettings_SetByConstructor_IsNotNull()
        {
            var service = new FakeService("username", "password", new HttpClient(), new WatsonSettings());
            Assert.IsNotNull(service);
            Assert.IsNotNull(service.Settings);
        }

        [TestMethod]
        public void WatsonSettings_SetByConstructor_AreEqual()
        {
            var client = new HttpClient { BaseAddress = new Uri("https://www.google.com") };
            var settings = new WatsonSettings
            {
                LearningOptOut = false
            };
            var service = new FakeService("username", "password", client, settings);
            Assert.AreEqual(service.Settings.LearningOptOut, false);
        }

        [TestMethod]
        public void HttpClient_SetByConstructor_AreEqual()
        {
            var client = new HttpClient { BaseAddress = new Uri("https://www.google.com") };
            var service = new FakeService("username", "password", client, new WatsonSettings());
            Assert.AreEqual(client.BaseAddress, service.HttpClient.BaseAddress);
        }

        [TestMethod]
        public void ApiKey_SetByConstructor_AreEqual()
        {
            var service = new FakeService("ausername", "apassword", new HttpClient(), new WatsonSettings());
            Assert.AreEqual("ausername:apassword", service.ApiKey);
        }

        [TestMethod]
        public void HttpClient_SetByConstructor_IsConfigured()
        {
            var service = new FakeService("ausername", "apassword", new HttpClient(), new WatsonSettings());

            var encoding = Encoding.UTF8;
            var byteArray = encoding.GetBytes(service.ApiKey);
            var apiKeyBase64 = Convert.ToBase64String(byteArray);
            var actualAuthorization = new AuthenticationHeaderValue("Basic", apiKeyBase64);

            //Assert that Authorization is set
            var expectedAuthorization = service.HttpClient.DefaultRequestHeaders.Authorization;
            Assert.AreEqual(expectedAuthorization.Parameter, actualAuthorization.Parameter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendRequestAsync_WithNullMessage_ThrowsArgumentNullException()
        {
            var service = new FakeService("ausername", "apassword", new HttpClient(), new WatsonSettings());
            await service.SendRequestAsync<bool>(null).ConfigureAwait(false);
        }
        
        [TestMethod]
        [ExpectedException(typeof(WatsonException))]
        public async Task SendRequestAsync_WithInvalidUrl_IsNull()
        {
            //Create a fake message handler
            var fakeHttpMessageHandler = new FakeHttpMessageHandler();
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("hello world")
            };

            //Add a fake response
            fakeHttpMessageHandler.AddFakeResponse("http://example.org/test", fakeResponse);

            //Create a HttpClient that will use the fake handler
            var httpClient = new HttpClient(fakeHttpMessageHandler);

            var service = new FakeService("ausername", "apassword", httpClient, new WatsonSettings());

            //Query a url we know doesn't exist in the fake handler
            var message = new HttpRequestMessage(HttpMethod.Get, "http://example.org/test2");
            var found = await service.SendRequestAsync<bool>(message).ConfigureAwait(false);

            Assert.IsFalse(found);
        }

        [TestMethod]
        [ExpectedException(typeof(WatsonException))]
        public async Task SendRequestAsync_With401ErrorMessage_AreEqual()
        {
            //Create a fake message handler
            var fakeHttpMessageHandler = new FakeHttpMessageHandler();
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(FakeErrors.Fake401Error)
            };

            fakeHttpMessageHandler.AddFakeResponse("http://example.org/test", fakeResponse);

            //Create a HttpClient that will use the fake handler
            var httpClient = new HttpClient(fakeHttpMessageHandler);

            //Inject the fake HttpClient when declaring a new service instance
            var service = new FakeService("ausername", "apassword", httpClient, new WatsonSettings());
            var message = new HttpRequestMessage(HttpMethod.Get, "http://example.org/test");
            var profile = await service.SendRequestAsync<bool>(message).ConfigureAwait(false);

            Assert.IsNotNull(profile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendRequestWithRawResponseAsync_WithNullMessage_ThrowsArgumentNullException()
        {
            var service = new FakeService("ausername", "apassword", new HttpClient(), new WatsonSettings());
            await service.SendRequestWithRawResponseAsync(null).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(WatsonException))]
        public async Task SendRequestWithRawResponseAsync_WithInvalidUrl_IsNull()
        {
            //Create a fake message handler
            var fakeHttpMessageHandler = new FakeHttpMessageHandler();
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("hello world")
            };

            //Add a fake response
            fakeHttpMessageHandler.AddFakeResponse("http://example.org/test", fakeResponse);

            //Create a HttpClient that will use the fake handler
            var httpClient = new HttpClient(fakeHttpMessageHandler);

            var service = new FakeService("ausername", "apassword", httpClient, new WatsonSettings());

            //Query a url we know doesn't exist in the fake handler
            var message = new HttpRequestMessage(HttpMethod.Get, "http://example.org/test2");
            var found = await service.SendRequestWithRawResponseAsync(message).ConfigureAwait(false);

            Assert.AreEqual("true", found);
        }

        [TestMethod]
        public async Task SendRequestWithRawResponseAsync_ReturnString_AreEqual()
        {
            //Create a fake message handler
            var fakeHttpMessageHandler = new FakeHttpMessageHandler();
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("hello world") };

            fakeHttpMessageHandler.AddFakeResponse("http://example.org/test", fakeResponse);

            //Create a HttpClient that will use the fake handler
            var httpClient = new HttpClient(fakeHttpMessageHandler);

            var service = new FakeService("ausername", "apassword", httpClient, new WatsonSettings());
            var message = new HttpRequestMessage(HttpMethod.Get, "http://example.org/test");
            var result = await service.SendRequestWithRawResponseAsync(message).ConfigureAwait(false);

            Assert.AreEqual("hello world", result);
        }

        [TestMethod]
        [ExpectedException(typeof(WatsonException))]
        public async Task SendRequestWithRawResponseAsync_With401ErrorMessage_AreEqual()
        {
            //Create a fake message handler
            var fakeHttpMessageHandler = new FakeHttpMessageHandler();
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(FakeErrors.Fake401Error)
            };

            fakeHttpMessageHandler.AddFakeResponse("http://example.org/test", fakeResponse);

            //Create a HttpClient that will use the fake handler
            var httpClient = new HttpClient(fakeHttpMessageHandler);

            //Inject the fake HttpClient when declaring a new service instance
            var service = new FakeService("ausername", "apassword", httpClient, new WatsonSettings());
            var message = new HttpRequestMessage(HttpMethod.Get, "http://example.org/test");
            var profile = await service.SendRequestWithRawResponseAsync(message).ConfigureAwait(false);

            Assert.IsNotNull(profile);
        }
    }
}