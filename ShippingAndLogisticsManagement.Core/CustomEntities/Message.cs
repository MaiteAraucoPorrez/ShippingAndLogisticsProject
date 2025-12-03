namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Represents a message with a type and description.
    /// </summary>
    /// <remarks>Use the <see cref="Message"/> class to encapsulate information about a message, including its
    /// category and descriptive text. This class is suitable for scenarios where messages need to be classified and
    /// described, such as logging, notifications, or status reporting.</remarks>
    public class Message
    {
        /// <summary>
        /// Gets or sets the type identifier associated with the object.
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// Gets or sets the descriptive text associated with the object.
        /// </summary>
        public string Description { get; set; }
    }

}
