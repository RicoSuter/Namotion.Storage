using System;

namespace Namotion.Storage.Abstractions
{
    public class BlobProperties
    {
        public BlobProperties(long length, DateTimeOffset? created = null, DateTimeOffset? lastModified = null, string eTag = null)
        {
            Length = length;
            Created = created;
            LastModified = lastModified;
            ETag = eTag;
        }

        public long Length { get; }

        public DateTimeOffset? Created { get; }

        public DateTimeOffset? LastModified { get; }

        public string ETag { get; }
    }
}
