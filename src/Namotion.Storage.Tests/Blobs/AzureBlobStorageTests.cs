using Microsoft.Extensions.Configuration;
using Namotion.Storage.Abstractions;
using Namotion.Storage.Azure.Storage.Blob;
using System.Threading.Tasks;
using Xunit;

namespace Namotion.Storage.Tests.Blobs
{
    public class AzureBlobStorageTests : BlobStorageTestsBase
    {
        protected override IBlobStorage CreateBlobStorage(IConfiguration configuration)
        {
            return AzureBlobStorage
                .CreateFromConnectionString(configuration["AzureBlobStorageConnectionString"]);
        }

        public override async Task<BlobElement> WhenWritingBlob_ThenPropertiesAreAvailable()
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
