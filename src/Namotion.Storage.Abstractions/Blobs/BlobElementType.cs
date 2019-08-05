namespace Namotion.Storage.Abstractions
{
    /// <summary>
    /// The blob entry types.
    /// </summary>
    public enum BlobElementType
    {
        /// <summary>
        /// A container where blobs can be stored.
        /// </summary>
        Container = 0,

        /// <summary>
        /// A blob.
        /// </summary>
        Blob = 1,
    }
}
