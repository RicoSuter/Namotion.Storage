using Namotion.Storage.Abstractions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    public class BlobStorage<T> : IBlobStorage<T>
    {
        private readonly IBlobStorage _blobStorage;

        public BlobStorage(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public virtual Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobStorage.DeleteAsync(path, cancellationToken);
        }

        public virtual Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobStorage.ExistsAsync(path, cancellationToken);
        }

        public virtual Task<BlobElement> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobStorage.GetAsync(path, cancellationToken);
        }

        public virtual Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobStorage.ListAsync(path, cancellationToken);
        }

        public virtual Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobStorage.OpenReadAsync(path, cancellationToken);
        }

        public virtual Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobStorage.OpenWriteAsync(path, cancellationToken);
        }

        public virtual Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobStorage.OpenAppendAsync(path, cancellationToken);
        }

        public virtual void Dispose()
        {
            _blobStorage.Dispose();
        }
    }
}
