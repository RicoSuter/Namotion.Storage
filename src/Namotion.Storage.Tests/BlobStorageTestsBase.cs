using Xunit;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
                    await container.WriteAllTextAsync(path, content);
                    var existsAfterWrite = await container.ExistsAsync(path);

                    var result = await container.ReadAllTextAsync(path);

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
        public virtual async Task<BlobElement> WhenWritingBlob_ThenElementPropertiesAreAvailable()
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                var path = Guid.NewGuid().ToString();

                try
                {
                    await container.WriteAllTextAsync(path, "Hello world!");

                    // Act
                    var element = await container.GetAsync(path);

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
        public virtual async Task WhenUpdatingMetdata_ThenMetadataIsStored()
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                var path = Guid.NewGuid().ToString();

                try
                {
                    await container.WriteAllTextAsync(path, "Hello world!");
                    var element = await container.GetAsync(path);

                    // Act
                    await container.UpdateMetadataAsync(path, new Dictionary<string, string> { { "foo", "bar"} });

                    // Assert
                    var updatedElement = await container.GetAsync(path);
                    Assert.Equal("bar", updatedElement.Metadata["foo"]);
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
                    await container.WriteAllTextAsync(path, content);

                    // Act
                    var result = await container.ReadAllTextAsync(path);

                    // Assert
                    Assert.Equal(content, result);
                }
                finally
                {
                    await container.DeleteAsync(path);
                }
            }
        }

        [Theory]
        [InlineData("doesnotexist")]
        [InlineData("does/not/exist")]
        public async Task WhenReadingNonExistingBlob_ThenExceptionIsThrown(string path)
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                // Assert
                await Assert.ThrowsAsync<BlobNotFoundException>(async () =>
                {
                    // Act
                    var result = await container.ReadAllTextAsync(path);
                });
            }
        }

        [Theory]
        [InlineData("doesnotexist")]
        [InlineData("does/not/exist")]
        public async Task WhenGettingNonExistingBlob_ThenExceptionIsThrown(string path)
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                // Assert
                await Assert.ThrowsAsync<BlobNotFoundException>(async () =>
                {
                    // Act
                    var result = await container.GetAsync(path);
                });
            }
        }

        [Theory]
        [InlineData("doesnotexist")]
        [InlineData("does/not/exist")]
        public async Task WhenListingNonExistingContainer_ThenExceptionIsThrown(string path)
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                // Assert
                await Assert.ThrowsAsync<ContainerNotFoundException>(async () =>
                {
                    // Act
                    var result = await container.ListAsync(path);
                });
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
                    await container.WriteJsonAsync(path, content);
                    var result = await container.ReadJsonAsync<string>(path);

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
        public async Task WhenWritingIntoObjectStorage_ThenObjectIsAvailable()
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                var objectStorage = container.CreateJsonObjectStorage<string>();

                var id = Guid.NewGuid().ToString();
                var content = Guid.NewGuid().ToString();

                try
                {
                    await objectStorage.WriteAsync(id, content);

                    // Act
                    var result = await objectStorage.ReadAsync(id);

                    // Assert
                    Assert.Equal(content, result);
                }
                finally
                {
                    await objectStorage.DeleteAsync(id);
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
                    await container.WriteJsonAsync(path, content);

                    // Act
                    var containers = await storage.ListAsync();
                    var items1 = await container.ListAsync();
                    var items2 = await container.ListAsync("foo");
                    var items3 = await container.ListAsync("foo/bar");

                    // Assert
                    Assert.Contains(containers, i => i.Name == "mystorage" && i.Type == BlobElementType.Container);
                    Assert.Contains(items1, i => i.Name == "foo" && i.Type == BlobElementType.Container);

                    Assert.Single(items2);
                    Assert.Contains(items2, i => i.Name == "bar" && i.Type == BlobElementType.Container);

                    Assert.Single(items2);
                    Assert.Contains(items3, i => i.Name == blobName && i.Type == BlobElementType.Blob);
                }
                finally
                {
                    await container.DeleteAsync(path);
                }
            }
        }

        [Fact]
        public virtual async Task WhenAppendingBlobOnly_ThenItHasBeenAppended()
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                var path = Guid.NewGuid().ToString();

                try
                {
                    // Act
                    await container.AppendTextAsync(path, "a");
                    await container.AppendTextAsync(path, "b");

                    var result = await container.ReadAllTextAsync(path);

                    // Assert
                    Assert.Equal("ab", result);
                }
                finally
                {
                    await container.DeleteAsync(path);
                }
            }
        }

        [Fact]
        public virtual async Task WhenWritingAndAppendingBlob_ThenItHasBeenAppended()
        {
            // Arrange
            var config = GetConfiguration();
            using (var container = GetBlobContainer(CreateBlobStorage(config)))
            {
                var path = Guid.NewGuid().ToString();

                try
                {
                    // Act
                    await container.WriteAllTextAsync(path, "a");
                    await container.AppendTextAsync(path, "b");

                    var result = await container.ReadAllTextAsync(path);

                    // Assert
                    Assert.Equal("ab", result);
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
