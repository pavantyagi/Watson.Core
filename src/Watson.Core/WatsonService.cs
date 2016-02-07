using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Watson.Core
{
    /// <summary>
    ///     Common Watson service functionality.
    /// </summary>
    public abstract class WatsonService : IWatsonService
    {
        /// <summary>
        ///     Initializes a new instance of the WatsonService class.
        /// </summary>
        internal WatsonService()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the WatsonService class.
        /// </summary>
        /// <param name="username">The service's username.</param>
        /// <param name="password">The service's password.</param>
        /// <param name="httpClient">The class for sending HTTP requests and receiving HTTP responses from the service methods.</param>
        /// <param name="settings">Common settings for all Watson Services.</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected WatsonService(string username, string password, HttpClient httpClient, WatsonSettings settings)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var apiKey = $"{username}:{password}";
            var encoding = Encoding.UTF8;
            var byteArray = encoding.GetBytes(apiKey);
            var apiKeyBase64 = Convert.ToBase64String(byteArray);
            var header = new AuthenticationHeaderValue("Basic", apiKeyBase64);

            httpClient.DefaultRequestHeaders.Authorization = header;
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Watson-Learning-Opt-Out",
                $"{settings.LearningOptOut}".ToLower());

            ApiKey = apiKey;
            HttpClient = httpClient;
            Settings = settings;
        }

        /// <summary>
        ///     The service's ApiKey (username:password).
        /// </summary>
        public virtual string ApiKey { get; }

        /// <summary>
        ///     The class for sending HTTP requests and receiving HTTP responses from the service methods.
        /// </summary>
        public virtual HttpClient HttpClient { get; }

        /// <summary>
        ///     Common settings for all Watson Services.
        /// </summary>
        public virtual WatsonSettings Settings { get; }

        /// <summary>
        ///     Send requests to the service and return a T response.
        /// </summary>
        /// <param name="message">The HttpRequestMessage that should be sent.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="WatsonException"></exception>
        /// <returns></returns>
        protected virtual async Task<T> SendRequestAsync<T>(HttpRequestMessage message)
        {
            var stringResponse = await SendRequestAsync(message).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(stringResponse))
                return default(T);

            return JsonConvert.DeserializeObject<T>(stringResponse);
        }

        /// <summary>
        ///     Send requests to the service and return the raw string response.
        /// </summary>
        /// <param name="message">The HttpRequestMessage that should be sent.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="WatsonException"></exception>
        /// <returns></returns>
        protected virtual async Task<string> SendRequestAsync(HttpRequestMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var httpResponse = await HttpClient.SendAsync(message).ConfigureAwait(false);

            string stringResponse = null;

            if (httpResponse.Content != null)
                stringResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            stringResponse = (stringResponse ?? string.Empty).Trim();

            //If not a 200 response parse the error response
            if (!httpResponse.IsSuccessStatusCode)
            {
                //Parse the error response if it's a valid json string
                if (stringResponse.StartsWith("{") && stringResponse.EndsWith("}") &&
                    stringResponse.Contains("error"))
                {
                    dynamic errorObject = JObject.Parse(stringResponse);
                    string error = errorObject.error;
                    throw new WatsonException(error);
                }

                //If the error can't be parsed, just return the reason phrase
                throw new WatsonException($"{(int) httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
            }

            return stringResponse;
        }
    }
}