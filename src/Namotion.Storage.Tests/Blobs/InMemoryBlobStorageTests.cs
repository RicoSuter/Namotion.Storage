using Microsoft.Extensions.Configuration;

namespace Namotion.Storage.Tests.Blobs
{
    public class InMemoryBlobStorageTests : BlobStorageTestsBase
    {
        protected override IBlobStorage CreateBlobStorage(IConfiguration configuration)
        {
            return new InMemoryBlobStorage();
        }
    }
}
