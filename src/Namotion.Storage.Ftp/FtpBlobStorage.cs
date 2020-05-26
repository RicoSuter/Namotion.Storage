﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;

namespace Namotion.Storage.Ftp
{
    public class FtpBlobStorage : IBlobStorage, IBlobContainer
    {
        private readonly FtpClient _client;

        private bool _loadPropertiesWithListing = false;

        private FtpBlobStorage(string host, int port, string username, string password)
        {
            _client = port != 0 ? new FtpClient(host) : new FtpClient(host, port, null, null);

            if (!string.IsNullOrEmpty(username))
            {
                _client.Credentials = new NetworkCredential(username, password);
            }
        }

        public static FtpBlobStorage Create(string host, string username, string password)
        {
            return new FtpBlobStorage(host, 0, username, password);
        }

        public static FtpBlobStorage Create(string host, int port, string username, string password)
        {
            return new FtpBlobStorage(host, port, username, password);
        }

        public async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            await _client.AutoConnectAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await _client.OpenReadAsync(path, FtpDataType.Binary, 0, true, cancellationToken).ConfigureAwait(false);
            }
            catch (FtpCommandException e) when (e.CompletionCode == "550")
            {
                throw new BlobNotFoundException(path, e);
            }
        }

        public async Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
        {
            await _client.AutoConnectAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await _client.OpenWriteAsync(path, FtpDataType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
            catch (FtpCommandException e) when (e.CompletionCode == "550")
            {
                var directory = Path.GetDirectoryName(path);
                await _client.CreateDirectoryAsync(directory, cancellationToken);
                return await _client.OpenWriteAsync(path, FtpDataType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default)
        {
            await _client.AutoConnectAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await _client.OpenAppendAsync(path, FtpDataType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
            catch (FtpCommandException e) when (e.CompletionCode == "550")
            {
                var directory = Path.GetDirectoryName(path);
                await _client.CreateDirectoryAsync(directory, cancellationToken);
                return await _client.OpenAppendAsync(path, FtpDataType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                await _client.AutoConnectAsync(cancellationToken).ConfigureAwait(false);
                await _client.DeleteFileAsync(path, cancellationToken).ConfigureAwait(false);
            }
            catch (FtpCommandException e) when (e.CompletionCode == "550")
            {
                // Ignore file does not exist
            }
        }

        public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            await _client.AutoConnectAsync(cancellationToken).ConfigureAwait(false);
            return await _client.FileExistsAsync(path, cancellationToken).ConfigureAwait(false);
        }

        public async Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default)
        {
            await _client.AutoConnectAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var items = await _client.GetListingAsync(path).ConfigureAwait(false);
                return items
                    .Select(i => i.Type == FtpFileSystemObjectType.Directory ?
                        BlobElement.CreateContainer(i.FullName, i.Name) :
                        new BlobElement(i.FullName, i.Name, BlobElementType.Blob, i.Size, i.Created.ToUniversalTime(), i.Modified.ToUniversalTime()))
                    .ToArray();
            }
            catch (FtpCommandException e) when (e.CompletionCode == "550")
            {
                throw new ContainerNotFoundException(path, e);
            }
        }

        public async Task<BlobElement> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            await _client.AutoConnectAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (_client.Capabilities.Contains(FtpCapability.MLSD) && !_loadPropertiesWithListing)
                {
                    try
                    {
                        var info = await _client.GetObjectInfoAsync(path, true).ConfigureAwait(false);
                        return new BlobElement(path, null, BlobElementType.Blob,
                            info.Size, info.Created.ToUniversalTime(), info.Modified.ToUniversalTime());
                    }
                    catch (PlatformNotSupportedException)
                    {
                        _loadPropertiesWithListing = true;
                        return await GetPropertiesWithListingAsync(path).ConfigureAwait(false);
                    }
                }
                else
                {
                    return await GetPropertiesWithListingAsync(path).ConfigureAwait(false);
                }
            }
            catch (FtpCommandException e) when (e.CompletionCode == "550")
            {
                throw new BlobNotFoundException(path, e);
            }
        }

        private async Task<BlobElement> GetPropertiesWithListingAsync(string path)
        {
            var name = Path.GetFileName(path);
            var directory = Path.GetDirectoryName(path);

            var items = await _client.GetListingAsync(directory).ConfigureAwait(false);
            var item = items.SingleOrDefault(i => i.Name == name);
            if (item == null)
            {
                throw new BlobNotFoundException(path, null);
            }

            return new BlobElement(path, null, BlobElementType.Blob,
                item.Size, item.Created.ToUniversalTime(), item.Modified.ToUniversalTime());
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
