using System;

namespace Namotion.Storage
{
    /// <summary>
    /// Thrown when the blob container does not exist.
    /// </summary>
    public class ContainerNotFoundException : Exception
    {
        public ContainerNotFoundException(string id, Exception innerException)
            : base("The container '" + id + "' does not exist.", innerException)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the requested, non existent blob ID.
        /// </summary>
        public string Id { get; }
    }
}
