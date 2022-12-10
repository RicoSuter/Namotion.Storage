using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage.Azure.Storage.Blob
{
    public class AzureBlobStorage : IBlobStorage
    {
        private readonly string _connectionString;

        private AzureBlobStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static IBlobStorage CreateFromConnectionString(string connectionString)
        {
            return new AzureBlobStorage(connectionString);
        }

        public async Task<BlobElement> GetAsync(string path, CancellationToken cancellationToken)
        {
            try
            {
                var blob = await GetBlockBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
                var properties = await blob.GetPropertiesAsync().ConfigureAwait(false);
                return new BlobElement(
                    path, null, BlobElementType.Blob,
                    properties.Value.ContentLength,
                    properties.Value.CreatedOn,
                    properties.Value.LastModified,
                    properties.Value.ETag.ToString(),
                    properties.Value.Metadata);
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                throw new BlobNotFoundException(path, e);
            }
        }

        public async Task UpdateMetadataAsync(string path, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            try
            {
                var blob = await GetBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
                await blob.SetMetadataAsync(metadata, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                throw new BlobNotFoundException(path, e);
            }
        }

        public async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var blob = await GetBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
                return await blob.OpenReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                throw new BlobNotFoundException(path, e);
            }
        }

        public async Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            var blob = await GetBlockBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
            return await blob.OpenWriteAsync(true, null, cancellationToken);
        }

        public async Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default)
        {
            var blob = await GetAppendBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
            return await blob.OpenWriteAsync(false, null, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            var blob = await GetBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
            return await blob.ExistsAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var blob = await GetBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
                await blob.DeleteAsync(DeleteSnapshotsOption.None, null, cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
            }
        }

        public async Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            var pathSegments = PathUtilities.GetSegments(path);
            if (pathSegments.Length == 0)
            {
                var service = new BlobServiceClient(_connectionString);

                var resultSegment = service
                    .GetBlobContainersAsync(BlobContainerTraits.Metadata, cancellationToken: cancellationToken)
                    .AsPages(default, 50);

                var results = new List<BlobElement>();
                await foreach (var containerPage in resultSegment.WithCancellation(cancellationToken))
                {
                    results.AddRange(containerPage.Values.Select(c => BlobElement.CreateContainer(c.Name)));
                }

                return results.ToArray();
            }
            else
            {
                var containerName = pathSegments.First();
                var containerPath = string.Join(PathUtilities.Delimiter, pathSegments.Skip(1));

                var blobContainerClient = new BlobContainerClient(_connectionString, containerName);
                var resultSegment = pathSegments.Skip(1).Any() ?
                    blobContainerClient
                        .GetBlobsByHierarchyAsync(delimiter: PathUtilities.Delimiter, prefix: containerPath + PathUtilities.Delimiter)
                        .AsPages(default, 50) :
                    blobContainerClient
                        .GetBlobsByHierarchyAsync(delimiter: PathUtilities.Delimiter)
                        .AsPages(default, 50);

                var results = new List<BlobElement>();
                await foreach (var blobPage in resultSegment.WithCancellation(cancellationToken))
                {
                    results.AddRange(blobPage.Values.Select(i =>
                    {
                        if (i.IsPrefix)
                        {
                            return BlobElement.CreateContainer(i.Prefix.TrimEnd('/'), PathUtilities.GetSegments(i.Prefix.TrimEnd('/')).Last());
                        }
                        else if (i.IsBlob)
                        {
                            var blobNameSegments = i.Blob.Name.Split(PathUtilities.DelimiterChar);
                            return new BlobElement(
                                i.Blob.Name,
                                string.Join(PathUtilities.Delimiter, blobNameSegments.Skip(blobNameSegments.Length - 1)),
                                BlobElementType.Blob,
                                i.Blob.Properties.ContentLength,
                                i.Blob.Properties.CreatedOn,
                                i.Blob.Properties.LastModified);
                        }
                        else
                        {
                            return null;
                        }
                    }).Where(c => c != null));
                }

                if (results.Count == 0)
                {
                    throw new ContainerNotFoundException(path, null);
                }

                return results.ToArray();
            }
        }

        public void Dispose()
        {
        }

        private async Task<BlobClient> GetBlobReferenceAsync(string path, CancellationToken cancellationToken)
        {
            var pathSegments = PathUtilities.GetSegments(path);
            var containerName = pathSegments.First();
            var blobName = string.Join(PathUtilities.Delimiter, pathSegments.Skip(1));

            var blobClient = new BlobClient(_connectionString, containerName, blobName);
            return blobClient;
        }

        private async Task<BlockBlobClient> GetBlockBlobReferenceAsync(string path, CancellationToken cancellationToken)
        {
            var pathSegments = PathUtilities.GetSegments(path);
            var containerName = pathSegments.First();
            var blobName = string.Join(PathUtilities.Delimiter, pathSegments.Skip(1));

            var blobClient = new BlockBlobClient(_connectionString, containerName, blobName);
            return blobClient;
        }

        private async Task<AppendBlobClient> GetAppendBlobReferenceAsync(string path, CancellationToken cancellationToken)
        {
            var pathSegments = PathUtilities.GetSegments(path);
            var containerName = pathSegments.First();
            var blobName = string.Join(PathUtilities.Delimiter, pathSegments.Skip(1));

            var blobClient = new AppendBlobClient(_connectionString, containerName, blobName);
            return blobClient;
        }
    }
}
