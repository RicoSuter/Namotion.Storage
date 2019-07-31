using Namotion.Storage.Abstractions;

namespace Namotion.Storage
{
    public static class IBlobStorageExtensions
    {
        public static IBlobContainer GetContainer(this IBlobStorage storage, string containerName)
        {
            return new BlobStorageToContainerAdapter(storage, containerName);
        }
    }
}
