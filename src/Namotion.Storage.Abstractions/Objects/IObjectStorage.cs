using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    /// <summary>
    /// An object storage.
    /// </summary>
    /// <typeparam name="T">The object value type.</typeparam>
    public interface IObjectStorage<T>
    {
        /// <summary>
        /// Writes an object to the storage.
        /// </summary>
        /// <param name="id">The object ID.</param>
        /// <param name="value">The object value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        Task WriteAsync(string id, T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads an object from the storage.
        /// </summary>
        /// <param name="id">The object ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The object value.</returns>
        Task<T> ReadAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an object from the storage.
        /// </summary>
        /// <param name="id">The object ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
