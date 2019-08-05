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

        public Task<BlobElement> GetElementAsync(string path, CancellationToken cancellationToken)
        {
            return Task.FromResult(new BlobElement(path, null, BlobElementType.Blob, _blobs[path].LongLength));
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_blobs.ContainsKey(path));
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                if (!_blobs.ContainsKey(path))
                {
                    throw new BlobNotFoundException(path, null);
                }

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

        public Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            var pathSegments = PathUtilities.GetSegments(path);
            return Task.FromResult(ListInternal(pathSegments)
                .GroupBy(i => i.Id)
                .Select(g => g.First())
                .Where(i => PathUtilities.GetSegments(i.Id).Length == pathSegments.Length + 1)
                .ToArray());
        }

        private IEnumerable<BlobElement> ListInternal(string[] pathSegments)
        {
            lock (_lock)
            {
                foreach (var blob in _blobs)
                {
                    var blobSegments = PathUtilities.GetSegments(blob.Key);
                    if (blobSegments.Length >= pathSegments.Length + 1)
                    {
                        if (blobSegments.Length == pathSegments.Length + 1 &&
                            blobSegments.Take(blobSegments.Length - 1).SequenceEqual(pathSegments))
                        {
                            yield return BlobElement.CreateBlob(blob.Key, blobSegments.Last());
                        }

                        for (var i = 1; i < blobSegments.Length; i++)
                        {
                            var path = string.Join("/", blobSegments.Take(i));
                            yield return BlobElement.CreateContainer(path, blobSegments.Skip(i - 1).First());
                        }
                    }
                }
            }
        }

        internal class InternalMemoryStream : MemoryStream
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
