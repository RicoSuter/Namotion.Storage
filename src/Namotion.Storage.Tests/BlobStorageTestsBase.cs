using Xunit;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Namotion.Storage.Abstractions;

namespace Namotion.Storage.Tests
{
    public abstract class BlobStorageTestsBase
    {
        [Fact]
        public async Task WhenWritingBlob_ThenItCanBeRead()
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                var path = Guid.NewGuid().ToString();
                var content = Guid.NewGuid().ToString();

                try
                {
                    // Act
                    var existsBeforeWrite = await container.ExistsAsync(path);
                    await container.WriteAsStringAsync(path, content);
                    var existsAfterWrite = await container.ExistsAsync(path);

                    var result = await container.ReadAsStringAsync(path);

                    await container.DeleteAsync(path);
                    var existsAfterDelete = await container.ExistsAsync(path);

                    // Assert
                    Assert.False(existsBeforeWrite);
                    Assert.Equal(content, result);
                    Assert.True(existsAfterWrite);
                    Assert.False(existsAfterDelete);
                }
                finally
                {
                    await container.DeleteAsync(path);
                }
            }
        }

        [Fact]
        public virtual async Task<BlobElement> WhenWritingBlob_ThenPropertiesAreAvailable()
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                var path = Guid.NewGuid().ToString();

                try
                {
                    await container.WriteAsStringAsync(path, "Hello world!");

                    // Act
                    var element = await container.GetElementAsync(path);

                    // Assert
                    Assert.True(element.Length > 0);
                    return element;
                }
                finally
                {
                    await container.DeleteAsync(path);
                }
            }
        }

        [Fact]
        public async Task WhenWritingBlobInDirectories_ThenItIsWritten()
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                var content = "Hello world!";
                var path = "dir1/dir2/" + Guid.NewGuid().ToString();

                try
                {
                    await container.WriteAsStringAsync(path, content);

                    // Act
                    var result = await container.ReadAsStringAsync(path);

                    // Assert
                    Assert.Equal(content, result);
                }
                finally
                {
                    await container.DeleteAsync(path);
                }
            }
        }

        [Fact]
        public async Task WhenWritingJsonBlob_ThenItCanBeRead()
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config))
                .WithBlobType<string>())
            {
                var path = Guid.NewGuid().ToString();
                var content = Guid.NewGuid().ToString();

                try
                {
                    // Act
                    await container.WriteAsJsonAsync(path, content);
                    var result = await container.ReadAsJsonAsync<string>(path);

                    // Assert
                    Assert.Equal(content, result);
                }
                finally
                {
                    await container.DeleteAsync(path);
                }
            }
        }

        [Fact]
        public async Task WhenNestedBlobIsCreated_ThenListingWorksAsExpected()
        {
            // Arrange
            var config = GetConfiguration();
            using (var storage = CreateBlobStorage(config))
            using (var container = GetBlobContainer(storage))
            {
                var blobName = Guid.NewGuid().ToString();
                var content = Guid.NewGuid().ToString();
                var path = "foo/bar/" + blobName;

                try
                {
                    await container.WriteAsJsonAsync(path, content);

                    // Act
                    var containers = await storage.ListAsync();
                    var items1 = await container.ListAsync();
                    var items2 = await container.ListAsync("foo");
                    var items3 = await container.ListAsync("foo/bar");

                    // Assert
                    Assert.Contains(containers, i => i.Name == "mystorage" && i.Type == BlobElementType.Container);
                    Assert.Contains(items1, i => i.Name == "foo" && i.Type == BlobElementType.Container);
                    Assert.Contains(items2, i => i.Name == "bar" && i.Type == BlobElementType.Container);
                    Assert.Contains(items3, i => i.Name == blobName && i.Type == BlobElementType.Blob);
                }
                finally
                {
                    await container.DeleteAsync(path);
                }
            }
        }

        protected IBlobContainer GetBlobContainer(IBlobStorage storage)
        {
            return storage.GetContainer("mystorage");
        }

        protected abstract IBlobStorage CreateBlobStorage(IConfiguration configuration);

        private static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
