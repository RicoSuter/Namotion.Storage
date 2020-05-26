using System;

namespace Namotion.Storage
{
    public interface IBlobStorage<T> : IBlobStorage { }

    /// <summary>
    /// A blob storage where blobs are stored in containers.
    /// Path parameters are in the form 'containerName/blobName' or 'containerName/subDirectories/blobName'.
    /// </summary>
    public interface IBlobStorage : IBlobReader, IBlobWriter, IDisposable
    {
    }
}
