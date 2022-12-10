using Microsoft.Extensions.Configuration;
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

        [Fact(Skip = "Not supported yet.")]
        public override Task WhenWritingAndAppendingBlob_ThenItHasBeenAppended()
        {
            return Task.CompletedTask;
        }

        public override async Task<BlobElement> WhenWritingBlob_ThenElementPropertiesAreAvailable()
        {
            var element = await base.WhenWritingBlob_ThenElementPropertiesAreAvailable();

            // Assert
            Assert.NotNull(element.Created);
            Assert.NotNull(element.LastModified);
            Assert.NotNull(element.ETag);

            return element;
        }
    }
}
