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
        protected WatsonService(string username, string password)
            : this(username, password, new HttpClient())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the WatsonService class.
        /// </summary>
        /// <param name="username">The service's username.</param>
        /// <param name="password">The service's password.</param>
        /// <param name="httpClient">The class for sending HTTP requests and receiving HTTP responses from the service methods.</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected WatsonService(string username, string password, HttpClient httpClient)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            var apiKey = $"{username}:{password}";
            var encoding = Encoding.UTF8;
            var byteArray = encoding.GetBytes(apiKey);
            var apiKeyBase64 = Convert.ToBase64String(byteArray);
            var header = new AuthenticationHeaderValue("Basic", apiKeyBase64);

            httpClient.DefaultRequestHeaders.Authorization = header;

            ApiKey = apiKey;
            HttpClient = httpClient;
        }

        /// <summary>
        ///     The service's ApiKey (username:password).
        /// </summary>
        public virtual string ApiKey { get; }

        /// <summary>
        ///     The Service's Url.
        /// </summary>
        public abstract string ServiceUrl { get; }

        /// <summary>
        ///     The class for sending HTTP requests and receiving HTTP responses from the service methods.
        /// </summary>
        public virtual HttpClient HttpClient { get; }

        /// <summary>
        ///     Send requests to the service.
        /// </summary>
        /// <param name="message">The HttpRequestMessage that should be sent.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        /// <exception cref="WatsonException"></exception>
        protected virtual async Task<T> SendRequestAsync<T>(HttpRequestMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var httpResponse = await HttpClient.SendAsync(message).ConfigureAwait(false);

            string stringResponse = null;

            if (httpResponse.Content != null)
                stringResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            stringResponse = stringResponse ?? string.Empty;

            //If not a 200 response
            if (!httpResponse.IsSuccessStatusCode)
            {
                //If the error can't be parsed, just return the reason phrase
                if (!stringResponse.StartsWith("{\"help"))
                    throw new WatsonException($"{(int) httpResponse.StatusCode} {httpResponse.ReasonPhrase}");

                //Parse the error
                dynamic errorObject = JObject.Parse(stringResponse);
                string error = errorObject.error.ToString();
                throw new WatsonException(error);
            }

            if (string.IsNullOrWhiteSpace(stringResponse))
                return default(T);

            //Certain services return text data (CSV etc)
            //So the return type should be string
            if (typeof (T) == typeof (string))
                return (T) Convert.ChangeType(stringResponse, typeof (string));

            return JsonConvert.DeserializeObject<T>(stringResponse);
        }
    }
}