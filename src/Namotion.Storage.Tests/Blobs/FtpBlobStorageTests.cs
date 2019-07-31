using Microsoft.Extensions.Configuration;
using Namotion.Storage.Abstractions;
using Namotion.Storage.Ftp;

namespace Namotion.Storage.Tests.Blobs
{
    public class FtpBlobStorageTests : BlobStorageTestsBase
    {
        protected override IBlobStorage CreateBlobStorage(IConfiguration configuration)
        {
            return FtpBlobStorage
                .Create("rsuter.com", 22, "test", configuration["FtpPassword"]);
        }
    }
}
