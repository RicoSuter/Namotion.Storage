using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Namotion.Storage.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage.Azure.Storage.Blob
{
    public class AzureBlobStorage : IBlobStorage
    {
        private readonly CloudStorageAccount _storageAccount;

        private AzureBlobStorage(CloudStorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
        }

        public static IBlobStorage CreateFromConnectionString(string connectionString)
        {
            return new AzureBlobStorage(CloudStorageAccount.Parse(connectionString));
        }

        public async Task<BlobElement> GetElementAsync(string path, CancellationToken cancellationToken)
        {
            var blob = await GetBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
            await blob.FetchAttributesAsync().ConfigureAwait(false);
            return new BlobElement(
                path, null, BlobElementType.Blob,
                blob.Properties.Length,
                blob.Properties.Created,
                blob.Properties.LastModified,
                blob.Properties.ETag);
        }

        private async Task<CloudBlockBlob> GetBlobReferenceAsync(string path, CancellationToken cancellationToken)
        {
            var pathSegments = PathUtilities.GetSegments(path);
            var containerName = pathSegments.First();
            var blobName = string.Join(PathUtilities.Delimiter, pathSegments.Skip(1));

            var container = await GetCloudBlobContainerAsync(containerName, cancellationToken).ConfigureAwait(false);
            return container.GetBlockBlobReference(blobName);
        }

        public async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            var blob = await GetBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
            return await blob.OpenReadAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            var blob = await GetBlobReferenceAsync(path, cancellationToken).ConfigureAwait(false);
            return await blob.OpenWriteAsync(cancellationToken).ConfigureAwait(false);
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
                await blob.DeleteAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (e.Message.Contains("does not exist."))
            {
            }
        }

        public async Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            var pathSegments = PathUtilities.GetSegments(path);
            if (pathSegments.Length == 0)
            {
                var cloudBlobClient = _storageAccount.CreateCloudBlobClient();

                BlobContinuationToken continuationToken = null;
                var results = new List<BlobElement>();
                do
                {
                    var response = await cloudBlobClient.ListContainersSegmentedAsync(continuationToken).ConfigureAwait(false);
                    continuationToken = response.ContinuationToken;
                    results.AddRange(response.Results.Select(c => BlobElement.CreateContainer(c.Name)));
                }
                while (continuationToken != null);

                return results.ToArray();
            }
            else
            {
                var containerName = pathSegments.First();
                var containerPath = string.Join(PathUtilities.Delimiter, pathSegments.Skip(1));
                var container = await GetCloudBlobContainerAsync(containerName, cancellationToken).ConfigureAwait(false);

                BlobContinuationToken continuationToken = null;
                var results = new List<BlobElement>();
                do
                {
                    var response = pathSegments.Skip(1).Any() ?
                        await container.ListBlobsSegmentedAsync(containerPath + PathUtilities.Delimiter, continuationToken).ConfigureAwait(false) :
                        await container.ListBlobsSegmentedAsync(continuationToken).ConfigureAwait(false);

                    continuationToken = response.ContinuationToken;
                    results.AddRange(response.Results.Select(i =>
                    {
                        if (i is CloudBlobDirectory directory)
                        {
                            return BlobElement.CreateContainer(directory.Prefix, PathUtilities.GetSegments(directory.Prefix).Last());
                        }
                        else if (i is CloudBlob blob)
                        {
                            var blobNameSegments = blob.Name.Split(PathUtilities.DelimiterChar);
                            return new BlobElement(
                                blob.Name,
                                string.Join(PathUtilities.Delimiter, blobNameSegments.Skip(blobNameSegments.Length - 1)),
                                BlobElementType.Blob,
                                blob.Properties.Length,
                                blob.Properties.Created,
                                blob.Properties.LastModified);
                        }
                        else
                        {
                            return null;
                        }
                    }).Where(c => c != null));
                }
                while (continuationToken != null);

                return results.ToArray();
            }
        }

        public void Dispose()
        {
        }

        private async Task<CloudBlobContainer> GetCloudBlobContainerAsync(string containerName, CancellationToken cancellationToken)
        {
            var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            if (await cloudBlobContainer.ExistsAsync(cancellationToken).ConfigureAwait(false) == false) // TODO: Create on throw and do not always check
            {
                await cloudBlobContainer.CreateAsync(cancellationToken).ConfigureAwait(false);
            }

            return cloudBlobContainer;
        }
    }
}
