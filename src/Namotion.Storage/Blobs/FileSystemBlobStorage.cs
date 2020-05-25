using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    /// <summary>
    /// A local file system based blob storage.
    /// </summary>
    public class FileSystemBlobStorage : IBlobStorage, IBlobContainer
    {
        private readonly string _basePath;

        private FileSystemBlobStorage(string basePath)
        {
            _basePath = basePath;
        }

        /// <summary>
        /// Creates a new storage with an abosulte base path.
        /// </summary>
        /// <param name="basePath">The absolute base path.</param>
        /// <returns>The blob storage or container.</returns>
        public static FileSystemBlobStorage CreateWithBasePath(string basePath)
        {
            return new FileSystemBlobStorage(basePath);
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            return Task.FromResult(File.Exists(fullPath));
        }

        public Task<BlobElement> GetAsync(string path, CancellationToken cancellationToken)
        {
            try
            {
                var fullPath = GetFullPath(path);
                return Task.FromResult(new BlobElement(
                    path, null, BlobElementType.Blob,
                    new FileInfo(fullPath).Length,
                    File.GetCreationTimeUtc(fullPath),
                    File.GetLastWriteTimeUtc(fullPath)));
            }
            catch (FileNotFoundException e)
            {
                throw new BlobNotFoundException(path, e);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new BlobNotFoundException(path, e);
            }
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullPath = GetFullPath(path);
                return Task.FromResult<Stream>(File.OpenRead(fullPath));
            }
            catch (FileNotFoundException e)
            {
                throw new BlobNotFoundException(path, e);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new BlobNotFoundException(path, e);
            }
        }

        public Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullPath = GetFullPath(path);
                return Task.FromResult<Stream>(File.Open(fullPath, FileMode.Append, FileAccess.Write));
            }
            catch (FileNotFoundException e)
            {
                throw new BlobNotFoundException(path, e);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new BlobNotFoundException(path, e);
            }
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

        public Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullPath = GetFullPath(path);

                var directories = Directory.GetDirectories(fullPath)
                    .Select(d => BlobElement.CreateContainer(d, Path.GetFileName(d)));

                var files = Directory.GetFiles(fullPath)
                    .Select(d => BlobElement.CreateBlob(d, Path.GetFileName(d)));

                return Task.FromResult(directories.Concat(files).ToArray());
            }
            catch (DirectoryNotFoundException e)
            {
                throw new ContainerNotFoundException(path, e);
            }           
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
