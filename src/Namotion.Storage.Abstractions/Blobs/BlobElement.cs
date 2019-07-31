using System;
using System.Linq;

namespace Namotion.Storage.Abstractions
{
    public class BlobElement
    {
        public BlobElement(string id,
            string name,
            BlobElementType type,
            long? length = null,
            DateTimeOffset? created = null,
            DateTimeOffset? lastModified = null,
            string eTag = null)
        {
            Id = id;
            Name = name ?? id.Split('/').Last();
            Type = type;
            Length = length;
            Created = created;
            LastModified = lastModified;
            ETag = eTag;
        }

        public static BlobElement CreateBlob(string id, string name)
        {
            return new BlobElement(id, name, BlobElementType.Blob);
        }

        public static BlobElement CreateContainer(string id, string name)
        {
            return new BlobElement(id, name, BlobElementType.Container);
        }

        public static BlobElement CreateContainer(string name)
        {
            return new BlobElement(name, name, BlobElementType.Blob);
        }

        /// <summary>
        /// Gets the absolute storage id of the element.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the relative name of the element.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the element type.
        /// </summary>
        public BlobElementType Type { get; private set; }

        /// <summary>
        /// Gets the blob length in bytes.
        /// </summary>
        public long? Length { get; }

        /// <summary>
        /// Gets the creation date time.
        /// </summary>
        public DateTimeOffset? Created { get; }

        /// <summary>
        /// Gets the last modification date time.
        /// </summary>
        public DateTimeOffset? LastModified { get; }

        /// <summary>
        /// Gets the blob ETag.
        /// </summary>
        public string ETag { get; }
    }
}
