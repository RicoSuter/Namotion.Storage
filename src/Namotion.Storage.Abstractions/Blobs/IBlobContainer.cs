using System;

namespace Namotion.Storage.Abstractions
{
    public interface IBlobContainer<T> : IBlobContainer { }

    /// <summary>
    /// A blob container where blobs are directly stored.
    /// Identifiers are in the form 'blobName' or 'subDirectory/blobName'.
    /// </summary>
    public interface IBlobContainer : IBlobReader, IBlobWriter, IDisposable
    {
    }
}
