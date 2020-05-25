using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    public static class IBlobReaderExtensions
    {
        public static Task<BlobElement[]> ListAsync(this IBlobReader blobReader, CancellationToken cancellationToken = default)
        {
            return blobReader.ListAsync(string.Empty, cancellationToken);
        }
    }
}
