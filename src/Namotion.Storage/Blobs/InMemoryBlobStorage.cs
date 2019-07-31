using Namotion.Storage.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    public class InMemoryBlobStorage : IBlobStorage
    {
        private readonly IDictionary<string, byte[]> _blobs;
        private readonly object _lock = new object();

        public InMemoryBlobStorage(IDictionary<string, byte[]> blobs = null)
        {
            _blobs = blobs ?? new Dictionary<string, byte[]>();
        }

        public Task<BlobProperties> GetPropertiesAsync(string path, CancellationToken cancellationToken)
        {
            return Task.FromResult(new BlobProperties(_blobs[path].LongLength));
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_blobs.ContainsKey(path));
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                return Task.FromResult<Stream>(new MemoryStream(_blobs[path].ToArray())
                {
                    Position = 0
                });
            }
        }

        public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Stream>(new InternalMemoryStream(this, path));
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                if (_blobs.ContainsKey(path))
                {
                    _blobs.Remove(path);
                }

                return Task.CompletedTask;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _blobs.Clear();
            }
        }

        public class InternalMemoryStream : MemoryStream
        {
            private readonly InMemoryBlobStorage _storage;
            private readonly string _identifier;

            public InternalMemoryStream(InMemoryBlobStorage storage, string identifier)
            {
                _storage = storage;
                _identifier = identifier;
            }

            protected override void Dispose(bool disposing)
            {
                lock (_storage._lock)
                {
                    _storage._blobs[_identifier] = ToArray();
                }

                base.Dispose(disposing);
            }
        }
    }
}
