using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    public class NullBlobStorage : IBlobStorage
    {
        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task UpdateMetadataAsync(string path, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<BlobElement> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<BlobElement>(null);
        }

        public Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new BlobElement[0]);
        }

        public Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Stream.Null);
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Stream.Null);
        }

        public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Stream.Null);
        }

        public void Dispose()
        {
        }
    }
}
