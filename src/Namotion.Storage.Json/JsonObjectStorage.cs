using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage.Json
{
    /// <summary>
    /// An object storage which stores object values as JSON in a blob container.
    /// </summary>
    /// <typeparam name="T">The object value type.</typeparam>
    public class JsonObjectStorage<T> : IObjectStorage<T>
    {
        private readonly IBlobContainer _blobContainer;

        internal JsonObjectStorage(IBlobContainer blobContainer)
        {
            _blobContainer = blobContainer;
        }

        /// <inheritdoc/>
        public async Task<T> ReadAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _blobContainer.ReadJsonAsync<T>(id, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task WriteAsync(string id, T value, CancellationToken cancellationToken = default)
        {
            await _blobContainer.WriteJsonAsync(id, value, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _blobContainer.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        }
    }
}
