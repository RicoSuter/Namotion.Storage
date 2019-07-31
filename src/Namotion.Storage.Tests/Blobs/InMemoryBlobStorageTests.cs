using Microsoft.Extensions.Configuration;
using Namotion.Storage.Abstractions;

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
