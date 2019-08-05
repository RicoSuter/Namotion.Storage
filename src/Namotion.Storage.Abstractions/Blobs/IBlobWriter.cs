using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage.Abstractions
{
    public interface IBlobWriter
    {
        /// <summary>
        /// Opens a stream to create or overwrite an entire blob.
        /// </summary>
        /// <param name="path">The blob path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The stream.</returns>
        Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Opens a stream to append data to a blob or creates a new one.
        /// </summary>
        /// <param name="path">The blob path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The stream.</returns>
        /// <exception cref="NotSupportedException">The method is not supported.</exception>
        Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a blob or does nothing if the blob does not exist.
        /// </summary>
        /// <param name="path">The blob path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        /// <exception cref="NotSupportedException">The method is not supported.</exception>
        Task DeleteAsync(string path, CancellationToken cancellationToken = default);
    }
}
