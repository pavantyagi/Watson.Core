using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Watson.Core.Tests.Fakes;

// ReSharper disable ExceptionNotDocumented

namespace Watson.Core.Tests.ServiceTests
{
    [TestClass]
    public class WatsonServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Username_SetNullByConstructor_ThrowArgumentNullException()
        {
            var service = new FakeService(null, "password", new HttpClient(), new WatsonSettings());
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Password_SetNullByConstructor_ThrowArgumentNullException()
        {
            var service = new FakeService("username", null, new HttpClient(), new WatsonSettings());
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void HttpClient_SetNullByConstructor_ThrowArgumentNullException()
        {
            var service = new FakeService("username", "password", null, new WatsonSettings());
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
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
            var client = new HttpClient {BaseAddress = new Uri("https://www.google.com")};
            var settings = new WatsonSettings
            {
                LearningOptOut = false
            };
            var service = new FakeService("username", "password", client, settings);
            Assert.AreEqual(settings.LearningOptOut, false);
        }

        [TestMethod]
        public void HttpClient_SetByConstructor_AreEqual()
        {
            var client = new HttpClient {BaseAddress = new Uri("https://www.google.com")};
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
        [ExpectedException(typeof (ArgumentNullException))]
        public async Task SendHttpRequestMessageAsync_WithNullMessage_ThrowsArgumentNullException()
        {
            var service = new FakeService("ausername", "apassword", new HttpClient(), new WatsonSettings());
            await service.SendRequestAsync<bool>(null).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof (WatsonException))]
        public async Task SendHttpRequestMessageAsync_WithInvalidUrl_IsNull()
        {
            //Create a fake message handler
            var fakeHttpMessageHandler = new FakeHttpMessageHandler();
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("hello world")
            };

            //Add a fake response
            fakeHttpMessageHandler.AddFakeResponse("http://example.org/test", fakeResponse);

            var service = new FakeService("ausername", "apassword", new HttpClient(fakeHttpMessageHandler),
                new WatsonSettings());

            //Query a url we know doesn't exist in the fake handler
            var message = new HttpRequestMessage(HttpMethod.Get, "http://example.org/test2");
            var profile = await service.SendRequestAsync<bool>(message).ConfigureAwait(false);

            Assert.IsNull(profile);
        }

        //public async Task SendHttpRequestMessageAsync_WithMessage_AreEqual()
        //{
        //    var constructorObjects = new object[3];
        //    constructorObjects[0] = "ausername";
        //    constructorObjects[1] = "apassword";

        //    //Create a fake message handler
        //    var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        //    var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        //    {
        //        Content = new StringContent(FakePersonalityInsightsResponses.FakeResponse)
        //    };

        //    fakeHttpMessageHandler.AddFakeResponse(new Uri("http://example.org/test"), fakeResponse);

        //[TestMethod]

        //    //Create a HttpClient that will use the fake handler
        //    var httpClient = new HttpClient(fakeHttpMessageHandler);

        //    //Inject the fake HttpClient when declaring a new service instance
        //    constructorObjects[2] = httpClient;

        //    var service = Substitute.ForPartsOf<WatsonService>(constructorObjects);
        //    var message = new HttpRequestMessage(HttpMethod.Get, "http://example.org/test");
        //    var profile = await service.SendHttpRequestMessageAsync<bool>(message);

        //    Assert.IsNotNull(profile);
        //}

        //[TestMethod]
        //public async Task SendHttpRequestMessageAsync_ReturnString_AreEqual()
        //{
        //    var constructorObjects = new object[3];
        //    constructorObjects[0] = "ausername";
        //    constructorObjects[1] = "apassword";

        //    //Create a fake message handler
        //    var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        //    var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("hello world") };

        //    fakeHttpMessageHandler.AddFakeResponse(new Uri("http://example.org/test"), fakeResponse);

        //    //Create a HttpClient that will use the fake handler
        //    var httpClient = new HttpClient(fakeHttpMessageHandler);

        //    //Inject the fake HttpClient when declaring a new service instance
        //    constructorObjects[2] = httpClient;

        //    var service = Substitute.ForPartsOf<WatsonService>(constructorObjects);
        //    var message = new HttpRequestMessage(HttpMethod.Get, "http://example.org/test");
        //    var profile = await service.SendHttpRequestMessageAsync<string>(message);

        //    Assert.IsNotNull(profile);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(WatsonException))]
        //public async Task SendHttpRequestMessageAsync_With401ErrorMessage_AreEqual()
        //{
        //    var constructorObjects = new object[3];
        //    constructorObjects[0] = "ausername";
        //    constructorObjects[1] = "apassword";

        //    //Create a fake message handler
        //    var fakeHttpMessageHandler = new FakeHttpMessageHandler();
        //    var fakeResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        //    {
        //        Content = new StringContent(FakeErrors.Fake401Error)
        //    };

        //    fakeHttpMessageHandler.AddFakeResponse(new Uri("http://example.org/test"), fakeResponse);

        //    //Create a HttpClient that will use the fake handler
        //    var httpClient = new HttpClient(fakeHttpMessageHandler);

        //    //Inject the fake HttpClient when declaring a new service instance
        //    constructorObjects[2] = httpClient;

        //    var service = Substitute.ForPartsOf<WatsonService>(constructorObjects);
        //    var message = new HttpRequestMessage(HttpMethod.Get, "http://example.org/test");
        //    var profile = await service.SendHttpRequestMessageAsync<bool>(message);

        //    Assert.IsNotNull(profile);
        //}

        //[TestMethod]
        //public void ParseError_ReturnString_AreEqual()
        //{
        //    dynamic errorObject = JObject.Parse(FakeErrors.Fake400Error);

        //    string error = errorObject.error.ToString();
        //    string code = errorObject.code.ToString();

        //    Assert.AreEqual(
        //        "The number of words 5 is less than the minimum number of words required for analysis: 100", error);

        //    Assert.AreEqual("400", code);
        //}
    }
}