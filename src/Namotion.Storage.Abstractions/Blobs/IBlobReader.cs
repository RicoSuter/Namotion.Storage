using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage.Abstractions
{
    public interface IBlobReader
    {
        Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

        Task<BlobElement> GetElementAsync(string path, CancellationToken cancellationToken = default);

        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);

        Task<BlobElement[]> ListAsync(string path, CancellationToken cancellationToken = default);
    }
}
