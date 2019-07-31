namespace Namotion.Storage.Abstractions
{
    public class BlobItem
    {
        private BlobItem(string path, bool isBlob, bool isContainer)
        {
            Path = path;
            IsBlob = isBlob;
            IsContainer = isContainer;
        }

        public static BlobItem CreateBlob(string path)
        {
            return new BlobItem(path, true, false);
        }

        public static BlobItem CreateContainer(string path)
        {
            return new BlobItem(path, false, true);
        }

        public string Path { get; }

        public bool IsBlob { get; }

        public bool IsContainer { get; }
    }
}
