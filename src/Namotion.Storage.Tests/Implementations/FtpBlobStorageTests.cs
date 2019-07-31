using Microsoft.Extensions.Configuration;
using Namotion.Storage.Tests;
using Namotion.Storage.Abstractions;
using Namotion.Storage.Ftp;

namespace Namotion.Messaging.Tests.Implementations
{
    public class FtpBlobStorageTests : StorageTestsBase
    {
        protected override IBlobContainer CreateBlobContainer(IConfiguration configuration)
        {
            return FtpBlobStorage.Create("rsuter.com", 22, "test", configuration["FtpPassword"]);
        }
    }
}
