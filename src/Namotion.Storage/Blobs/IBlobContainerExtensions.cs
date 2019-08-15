using Namotion.Storage.Abstractions;

namespace Namotion.Storage
{
    public static class IBlobContainerExtensions
    {
        public static IBlobContainer<T> WithBlobType<T>(this IBlobContainer blobContainer)
        {
            return new BlobContainer<T>(blobContainer);
        }
    }
}
