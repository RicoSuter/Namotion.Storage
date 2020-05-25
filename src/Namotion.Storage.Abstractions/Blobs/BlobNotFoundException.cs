using System;

namespace Namotion.Storage
{
    /// <summary>
    /// Thrown when the blob does not exist.
    /// </summary>
    public class BlobNotFoundException : Exception
    {
        public BlobNotFoundException(string id, Exception innerException)
            : base("The blob '" + id + "' does not exist.", innerException)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the requested, non existent blob ID.
        /// </summary>
        public string Id { get; }
    }
}
