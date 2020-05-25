namespace Namotion.Storage
{
    public static class IBlobContainerExtensions
    {
        /// <summary>
        /// Adds a generic blob type to the blob container.
        /// </summary>
        /// <typeparam name="T">The blob type.</typeparam>
        /// <param name="blobContainer">The blob container.</param>
        /// <returns>The wrapped blob container.</returns>
        public static IBlobContainer<T> WithBlobType<T>(this IBlobContainer blobContainer)
        {
            return new BlobContainer<T>(blobContainer);
        }
    }
}
