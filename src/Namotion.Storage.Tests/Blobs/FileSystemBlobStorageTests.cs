﻿using Microsoft.Extensions.Configuration;
using Namotion.Storage.Abstractions;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Namotion.Storage.Tests.Blobs
{
    public class FileSystemBlobStorageTests : BlobStorageTestsBase
    {
        public override async Task<BlobProperties> WhenWritingBlob_ThenPropertiesAreAvailable()
        {
            var properties = await base.WhenWritingBlob_ThenPropertiesAreAvailable();

            // Assert
            Assert.NotNull(properties.Created);
            Assert.NotNull(properties.LastModified);

            return properties;
        }

        protected override IBlobStorage CreateBlobStorage(IConfiguration configuration)
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return FileSystemBlobStorage.CreateWithBasePath(tempDirectory);
        }
    }
}