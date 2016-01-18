using System.Net.Http;

namespace Watson.Core
{
    /// <summary>
    ///     Common Watson service functionality.
    /// </summary>
    public interface IWatsonService
    {
        /// <summary>
        ///     The service's ApiKey (username:password).
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        ///     The Service's Url.
        /// </summary>
        string ServiceUrl { get; }

        /// <summary>
        ///     The class for sending HTTP requests and receiving HTTP responses from the service methods.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        ///     Common settings for all Watson Services.
        /// </summary>
        WatsonSettings Settings { get; }
    }
}