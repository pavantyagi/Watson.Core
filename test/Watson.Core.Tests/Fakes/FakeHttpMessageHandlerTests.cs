using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable ExceptionNotDocumented

namespace Watson.Core.Tests.Fakes
{
    public class FakeHttpMessageHandlerTests
    {
        public static Dictionary<string, string> PostContent => new Dictionary<string, string>
        {
            {"Name", "bob"},
            {"Address", "Ireland"},
            {"Phone", "12345"}
        };

        [Theory]
        [InlineData("http://example.org/exists", "http://example.org/exists", "Hello World", false)]
        [InlineData("http://example.org/exists", "http://example.org/notexists", "Hello World", true)]
        public async Task HttpClient_GetAsync(string requestUrl, string contentUrl, string contentAtUrl,
            bool contentIsNull)
        {
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(contentAtUrl)};

            var httpClient = new HttpClient(new FakeHttpMessageHandler(contentUrl, fakeResponse));
            var httpResponse = await httpClient.GetAsync(requestUrl).ConfigureAwait(false);
            string stringResponse = null;

            if (httpResponse.Content != null)
                stringResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.Equal(string.IsNullOrEmpty(stringResponse), contentIsNull);

            if (!contentIsNull)
                Assert.Equal(contentAtUrl, stringResponse);
        }

        [Theory]
        [InlineData("http://example.org/exists", "http://example.org/exists", "accepted=true", false)]
        [InlineData("http://example.org/exists", "http://example.org/notexists", "error", true)]
        public async Task HttpClient_PostAsync(string requestUrl, string contentUrl, string contentAtUrl,
            bool contentIsNull)
        {
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(contentAtUrl)};

            var httpClient = new HttpClient(new FakeHttpMessageHandler(contentUrl, fakeResponse));
            var content = new FormUrlEncodedContent(PostContent.ToArray());
            var httpResponse = await httpClient.PostAsync(requestUrl, content).ConfigureAwait(false);
            string stringResponse = null;

            if (httpResponse.Content != null)
                stringResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.Equal(string.IsNullOrEmpty(stringResponse), contentIsNull);

            if (!contentIsNull)
                Assert.Equal(contentAtUrl, stringResponse);
        }
    }
}