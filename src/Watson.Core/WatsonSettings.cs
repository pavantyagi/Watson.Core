namespace Watson.Core
{
    /// <summary>
    ///     Common settings for all Watson Services.
    /// </summary>
    public class WatsonSettings
    {
        /// <summary>
        ///     Opt out of Watson using your data to improve the services. Default: true
        /// </summary>
        public bool LearningOptOut { get; set; } = true;
    }
}