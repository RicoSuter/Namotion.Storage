using Microsoft.Extensions.Configuration;
using Namotion.Storage;
using Namotion.Storage.Abstractions;
using Namotion.Storage.Tests;

namespace Namotion.Messaging.Tests.Implementations
{
    public class InMemoryBlobStorageTests : StorageTestsBase
    {
        protected override IBlobContainer CreateBlobContainer(IConfiguration configuration)
        {
            return new InMemoryBlobStorage().GetContainer("mystorage");
        }
    }
}
