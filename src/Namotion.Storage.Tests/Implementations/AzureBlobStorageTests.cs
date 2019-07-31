using Microsoft.Extensions.Configuration;
using Namotion.Storage;
using Namotion.Storage.Tests;
using Namotion.Storage.Abstractions;
using Namotion.Storage.Azure.Storage.Blob;
using System.Threading.Tasks;
using Xunit;

namespace Namotion.Messaging.Tests.Implementations
{
    public class AzureBlobStorageTests : StorageTestsBase
    {
        protected override IBlobContainer CreateBlobContainer(IConfiguration configuration)
        {
            return AzureBlobStorage
                .CreateFromConnectionString(configuration["AzureBlobStorageConnectionString"])
                .GetContainer("mystorage");
        }

        public override async Task<BlobProperties> WhenWritingBlob_ThenPropertiesAreAvailable()
        {
            var properties = await base.WhenWritingBlob_ThenPropertiesAreAvailable();

            // Assert
            Assert.NotNull(properties.Created);
            Assert.NotNull(properties.LastModified);
            Assert.NotNull(properties.ETag);

            return properties;
        }
    }
}
