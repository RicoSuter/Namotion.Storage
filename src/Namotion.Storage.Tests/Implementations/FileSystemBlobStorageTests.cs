using Microsoft.Extensions.Configuration;
using Namotion.Storage;
using Namotion.Storage.Tests;
using Namotion.Storage.Abstractions;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Namotion.Messaging.Tests.Implementations
{
    public class FileSystemBlobStorageTests : StorageTestsBase
    {
        protected override IBlobContainer CreateBlobContainer(IConfiguration configuration)
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return FileSystemBlobStorage.CreateWithBasePath(tempDirectory);
        }

        public override async Task<BlobProperties> WhenWritingBlob_ThenPropertiesAreAvailable()
        {
            var properties = await base.WhenWritingBlob_ThenPropertiesAreAvailable();

            // Assert
            Assert.NotNull(properties.Created);
            Assert.NotNull(properties.LastModified);

            return properties;
        }
    }
}
