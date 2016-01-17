using System;

namespace Watson.Core
{
    /// <summary>
    ///     The exception that is thrown when an error is found in a response from a Watson service.
    /// </summary>
    public class WatsonException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the WatsonException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WatsonException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the WatsonException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public WatsonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}