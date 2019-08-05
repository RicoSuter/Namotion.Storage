using Namotion.Storage.Abstractions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    public class BlobContainer<T> : IBlobContainer<T>
    {
        private readonly IBlobContainer _blobContainer;

        public BlobContainer(IBlobContainer blobContainer)
        {
            _blobContainer = blobContainer;
        }

        public virtual Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobContainer.DeleteAsync(path, cancellationToken);
        }

        public virtual Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobContainer.ExistsAsync(path, cancellationToken);
        }

        public virtual Task<BlobElement> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobContainer.GetAsync(path, cancellationToken);
        }

        public virtual Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobContainer.ListAsync(path, cancellationToken);
        }

        public virtual Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobContainer.OpenReadAsync(path, cancellationToken);
        }

        public virtual Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobContainer.OpenWriteAsync(path, cancellationToken);
        }

        public virtual Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default)
        {
            return _blobContainer.OpenAppendAsync(path, cancellationToken);
        }

        public virtual void Dispose()
        {
            _blobContainer.Dispose();
        }
    }
}
