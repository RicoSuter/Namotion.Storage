using Xunit;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Namotion.Storage.Abstractions;

namespace Namotion.Storage.Tests
{
    public abstract class StorageTestsBase
    {
        [Fact]
        public async Task WhenWritingBlob_ThenItCanBeRead()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var container = CreateBlobContainer(config);
            var path = Guid.NewGuid().ToString();
            var content = Guid.NewGuid().ToString();

            try
            {
                // Act
                var existsBeforeWrite = await container.ExistsAsync(path);
                await container.WriteAsStringAsync(path, content);
                var result = await container.ReadAsStringAsync(path);
                var existsAfterWrite = await container.ExistsAsync(path);
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

        [Fact]
        public virtual async Task<BlobProperties> WhenWritingBlob_ThenPropertiesAreAvailable()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var container = CreateBlobContainer(config);
            var path = Guid.NewGuid().ToString();

            try
            {
                await container.WriteAsStringAsync(path, "Hello world!");

                // Act
                var properties = await container.GetPropertiesAsync(path);

                // Assert
                Assert.True(properties.Length > 0);
                return properties;
            }
            finally
            {
                await container.DeleteAsync(path);
            }
        }

        [Fact]
        public async Task WhenWritingBlobInDirectories_ThenItIsWritten()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var container = CreateBlobContainer(config);
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

        [Fact]
        public async Task WhenWritingJsonBlob_ThenItCanBeRead()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var container = CreateBlobContainer(config);
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

        protected abstract IBlobContainer CreateBlobContainer(IConfiguration configuration);
    }
}
