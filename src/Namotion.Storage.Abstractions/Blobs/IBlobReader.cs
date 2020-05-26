using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    public interface IBlobReader
    {
        /// <summary>
        /// Checks whether a blob exists.
        /// </summary>
        /// <param name="path">The blob path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True when the blob exists.</returns>
        Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the blob element description.
        /// </summary>
        /// <param name="path">The blob path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The blob element.</returns>
        /// <exception cref="BlobNotFoundException">The blob does not exist.</exception>
        /// <exception cref="NotSupportedException">The method is not supported.</exception>
        Task<BlobElement> GetAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a read stream for the given blob.
        /// </summary>
        /// <param name="path">The blob path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The stream.</returns>
        /// <exception cref="BlobNotFoundException">The blob does not exist.</exception>
        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of all blobs and containers in the given path with the name relative the the input path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The blobs and container element descriptions.</returns>
        /// <exception cref="NotSupportedException">The method is not supported.</exception>
        Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default);
    }
}
