using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage.Abstractions
{
    public interface IBlobReader
    {
        Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

        /// <exception cref="NotSupportedException">The method is not supported.</exception>
        Task<BlobElement> GetElementAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a read stream for the given blob.
        /// </summary>
        /// <param name="path">The blob path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The stream.</returns>
        /// <exception cref="BlobNotFoundException">The blob does not exist.</exception>
        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);

        /// <exception cref="NotSupportedException">The method is not supported.</exception>
        Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default);
    }
}
