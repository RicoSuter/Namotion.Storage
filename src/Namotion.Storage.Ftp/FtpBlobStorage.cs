using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Namotion.Storage.Abstractions;

namespace Namotion.Storage.Ftp
{
    public class FtpBlobStorage : IBlobStorage, IBlobContainer
    {
        private readonly FtpClient _client;

        private FtpBlobStorage(string host, string username, string password)
        {
            _client = new FtpClient(host);

            if (!string.IsNullOrEmpty(username))
            {
                _client.Credentials = new NetworkCredential(username, password);
            }
        }

        public static FtpBlobStorage Create(string host, string username, string password)
        {
            return new FtpBlobStorage(host, username, password);
        }

        public async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            await _client.AutoConnectAsync(cancellationToken).ConfigureAwait(false);
            return await _client.OpenReadAsync(path, FtpDataType.Binary, 0, true, cancellationToken).ConfigureAwait(false);
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

        public async Task<BlobProperties> GetPropertiesAsync(string path, CancellationToken cancellationToken = default)
        {
            await _client.AutoConnectAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var info = await _client.GetObjectInfoAsync(path, true).ConfigureAwait(false);
                return new BlobProperties(info.Size, info.Created.ToUniversalTime(), info.Modified.ToUniversalTime());
            }
            catch (PlatformNotSupportedException)
            {
                var fileSizeTask = _client.GetFileSizeAsync(path, cancellationToken).ConfigureAwait(false);
                var modifiedTimeTask = _client.GetModifiedTimeAsync(path, FtpDate.UTC, cancellationToken).ConfigureAwait(false);
                return new BlobProperties(await fileSizeTask, null, await modifiedTimeTask);
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
