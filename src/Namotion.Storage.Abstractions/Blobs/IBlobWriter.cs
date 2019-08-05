using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage.Abstractions
{
    public interface IBlobWriter
    {
        Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default);

        /// <exception cref="NotSupportedException">The method is not supported.</exception>
        //Task<Stream> OpenAppendAsync(string path, CancellationToken cancellationToken = default);

        /// <exception cref="NotSupportedException">The method is not supported.</exception>
        Task DeleteAsync(string path, CancellationToken cancellationToken = default);
    }
}
