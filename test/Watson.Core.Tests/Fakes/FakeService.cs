using System.Net.Http;
using System.Threading.Tasks;

// ReSharper disable ExceptionNotDocumented

namespace Watson.Core.Tests.Fakes
{
    public class FakeService : WatsonService
    {
        public FakeService(string username, string password) : base(username, password)
        {
        }

        public FakeService(string username, string password, HttpClient httpClient)
            : base(username, password, httpClient)
        {
        }

        public override string ServiceUrl { get; } = "https://gateway.watsonplatform.net/";

        public new async Task<T> SendRequestAsync<T>(HttpRequestMessage message)
        {
            return await base.SendRequestAsync<T>(message).ConfigureAwait(false);
        }
    }
}