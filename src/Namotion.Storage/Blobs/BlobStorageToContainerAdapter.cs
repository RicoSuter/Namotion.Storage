using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    internal class BlobStorageToContainerAdapter : IBlobContainer
    {
        private readonly IBlobStorage _storage;
        private readonly string _containerName;

        public BlobStorageToContainerAdapter(IBlobStorage storage, string containerName)
        {
            _storage = storage;
            _containerName = containerName;
        }

        public Task<BlobElement> GetAsync(string path, CancellationToken cancellationToken)
        {
            return _storage.GetAsync(_containerName + "/" + path, cancellationToken);
        }

        public Task UpdateMetadataAsync(string path, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            return _storage.UpdateMetadataAsync(_containerName + "/" + path, metadata, cancellationToken);
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            return _storage.ExistsAsync(_containerName + "/" + path, cancellationToken);
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            return _storage.OpenReadAsync(_containerName + "/" + path, cancellationToken);
        }

        public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            return _storage.OpenWriteAsync(_containerName + "/" + path, cancellationToken);
        }

        public Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default)
        {
            return _storage.OpenAppendAsync(_containerName + "/" + path, cancellationToken);
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            return _storage.DeleteAsync(_containerName + "/" + path, cancellationToken);
        }

        public Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            return _storage.ListAsync(_containerName + "/" + path, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}
