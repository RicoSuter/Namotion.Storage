using System.Collections.Generic;
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
        private readonly IReadOnlyDictionary<string, string> _pathReplacements;

        /// <summary>
        /// Creates a new instance of the file system based blob storage.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="pathReplacements">The path replacements.</param>
        protected FileSystemBlobStorage(string basePath, IReadOnlyDictionary<string, string> pathReplacements = null)
        {
            _basePath = basePath;
            _pathReplacements = pathReplacements ?? new Dictionary<string, string> { { ":", "%3A" } };
        }

        /// <summary>
        /// Creates a new storage with an abosulte base path.
        /// </summary>
        /// <param name="basePath">The absolute base path.</param>
        /// <param name="pathReplacements">The path replacements or use defaults (':' => '%3A').</param>
        /// <returns>The blob storage or container.</returns>
        public static FileSystemBlobStorage CreateWithBasePath(string basePath, IReadOnlyDictionary<string, string> pathReplacements = null)
        {
            return new FileSystemBlobStorage(basePath, pathReplacements);
        }

        /// <inheritdoc />>
        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            return Task.FromResult(File.Exists(fullPath));
        }

        /// <inheritdoc />>
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

        /// <inheritdoc />>
        public Task UpdateMetadataAsync(string path, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />>
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

        /// <inheritdoc />>
        public Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullPath = GetFullPath(path);
                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

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

        /// <inheritdoc />>
        public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return Task.FromResult<Stream>(File.Open(fullPath, FileMode.Create, FileAccess.Write));
        }

        /// <inheritdoc />>
        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            File.Delete(fullPath);
            return Task.CompletedTask;
        }

        /// <inheritdoc />>
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

        /// <inheritdoc />>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets the full path based on the identifier.
        /// </summary>
        /// <param name="identifier">The item identifier.</param>
        /// <returns>The full path.</returns>
        protected virtual string GetFullPath(string identifier)
        {
            foreach (var pathReplacement in _pathReplacements)
            {
                identifier = identifier.Replace(pathReplacement.Key, pathReplacement.Value);
            }

            return Path.Combine(_basePath, identifier);
        }
    }
}
