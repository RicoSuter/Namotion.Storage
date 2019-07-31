using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage.Abstractions
{
    public interface IBlobWriter
    {
        Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default);

        Task DeleteAsync(string path, CancellationToken cancellationToken = default);
    }
}
