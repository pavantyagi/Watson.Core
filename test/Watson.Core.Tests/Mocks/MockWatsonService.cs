using System.Net.Http;
using System.Threading.Tasks;

// ReSharper disable ExceptionNotDocumented

namespace Watson.Core.Tests.Mocks
{
    public class MockWatsonService : WatsonService
    {
        public MockWatsonService(string username, string password, HttpClient httpClient, WatsonSettings settings)
            : base(username, password, httpClient, settings)
        {
        }

        public new async Task<T> SendRequestAsync<T>(HttpRequestMessage message)
        {
            return await base.SendRequestAsync<T>(message).ConfigureAwait(false);
        }

        public new async Task<string> SendRequestAsync(HttpRequestMessage message)
        {
            return await base.SendRequestAsync(message).ConfigureAwait(false);
        }
    }
}