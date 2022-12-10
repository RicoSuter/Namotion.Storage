using Microsoft.Extensions.Configuration;
using Namotion.Storage.Ftp;
using System.Threading.Tasks;
using Xunit;

namespace Namotion.Storage.Tests.Blobs
{
    public class FtpBlobStorageTests : BlobStorageTestsBase
    {
        protected override IBlobStorage CreateBlobStorage(IConfiguration configuration)
        {
            return FtpBlobStorage
                .Create("rsuter.com", 22, "test", configuration["FtpPassword"]);
        }

        [Fact(Skip = "Not supported yet.")]
        public override Task WhenAppendingBlobOnly_ThenItHasBeenAppended()
        {
            return Task.CompletedTask;
        }

        [Fact(Skip = "Test FTP server does not support append.")]
        public override Task WhenWritingAndAppendingBlob_ThenItHasBeenAppended()
        {
            return Task.CompletedTask;
        }

        [Fact(Skip = "FTP does not support metadata.")]
        public override Task WhenUpdatingMetdata_ThenMetadataIsStored()
        {
            return Task.CompletedTask;
        }
    }
}
