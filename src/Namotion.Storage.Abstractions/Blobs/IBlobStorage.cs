using System;

namespace Namotion.Storage.Abstractions
{
    /// <summary>
    /// A blob storage where blobs are stored in containers.
    /// Identifiers are in the form 'containerName/blobName' or 'containerName/subDirectory/blobName'.
    /// </summary>
    public interface IBlobStorage : IBlobReader, IBlobWriter, IDisposable
    {
    }
}
