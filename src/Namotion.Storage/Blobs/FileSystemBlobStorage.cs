using Namotion.Storage.Abstractions;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    public class FileSystemBlobStorage : IBlobStorage, IBlobContainer
    {
        private readonly string _basePath;

        private FileSystemBlobStorage(string basePath)
        {
            _basePath = basePath;
        }

        public static FileSystemBlobStorage CreateWithBasePath(string basePath)
        {
            return new FileSystemBlobStorage(basePath);
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            return Task.FromResult(File.Exists(fullPath));
        }

        public Task<BlobProperties> GetPropertiesAsync(string path, CancellationToken cancellationToken)
        {
            var fullPath = GetFullPath(path);
            return Task.FromResult(new BlobProperties(
                new FileInfo(fullPath).Length,
                File.GetCreationTimeUtc(fullPath),
                File.GetLastWriteTimeUtc(fullPath)));
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            return Task.FromResult<Stream>(File.OpenRead(fullPath));
        }

        public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return Task.FromResult<Stream>(File.OpenWrite(fullPath));
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            File.Delete(fullPath);
            return Task.CompletedTask;
        }

        public Task<BlobItem[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);

            var directories = Directory.GetDirectories(fullPath)
                .Select(d => BlobItem.CreateContainer(d, Path.GetFileName(d)));

            var files = Directory.GetFiles(fullPath)
                .Select(d => BlobItem.CreateBlob(d, Path.GetFileName(d)));

            return Task.FromResult(directories.Concat(files).ToArray());
        }

        public void Dispose()
        {
        }

        protected virtual string GetFullPath(string identifier)
        {
            return Path.Combine(_basePath, identifier);
        }
    }
}
