using Namotion.Storage.Abstractions;

namespace Namotion.Storage
{
    /// <summary>
    /// Container based blob storage extension methods.
    /// </summary>
    public static class IBlobStorageExtensions
    {
        /// <summary>
        /// Gets a container inside the storage.
        /// The container path must have at least one segment, e.g. 'foo' or 'foo/bar'.
        /// </summary>
        /// <param name="storage">The blob storage.</param>
        /// <param name="path">The container path.</param>
        /// <returns></returns>
        public static IBlobContainer GetContainer(this IBlobStorage storage, string path)
        {
            return new BlobStorageToContainerAdapter(storage, path);
        }

        /// <summary>
        /// Adds a generic blob type to the blob storage.
        /// </summary>
        /// <typeparam name="T">The blob type.</typeparam>
        /// <param name="blobStorage">The blob storage.</param>
        /// <returns>The wrapped blob storage.</returns>
        public static IBlobStorage<T> WithBlobType<T>(this IBlobStorage blobStorage)
        {
            return new BlobStorage<T>(blobStorage);
        }
    }
}
