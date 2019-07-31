using Namotion.Storage.Abstractions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    public static class StringBlobExtensions
    {
        public static async Task WriteAsStringAsync(this IBlobWriter writer, string path, string value, CancellationToken cancellationToken = default)
        {
            using (var streamWriter = new StreamWriter(await writer.OpenWriteAsync(path, cancellationToken).ConfigureAwait(false)))
            {
                await streamWriter.WriteAsync(value).ConfigureAwait(false);
            }
        }

        public static async Task<string> ReadAsStringAsync(this IBlobReader reader, string path, CancellationToken cancellationToken = default)
        {
            using (var streamReader = new StreamReader(await reader.OpenReadAsync(path, cancellationToken).ConfigureAwait(false)))
            {
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}
