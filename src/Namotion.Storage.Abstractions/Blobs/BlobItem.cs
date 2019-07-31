namespace Namotion.Storage.Abstractions
{
    public class BlobItem
    {
        private BlobItem(string id, string name, bool isBlob, bool isContainer)
        {
            Id = id;
            Name = name;
            IsBlob = isBlob;
            IsContainer = isContainer;
        }

        public static BlobItem CreateBlob(string id, string name)
        {
            return new BlobItem(id, name, true, false);
        }

        public static BlobItem CreateContainer(string id, string name)
        {
            return new BlobItem(id, name, false, true);
        }

        public static BlobItem CreateContainer(string name)
        {
            return new BlobItem(name, name, false, true);
        }

        public string Id { get; }

        public string Name { get; }

        public bool IsBlob { get; }

        public bool IsContainer { get; }
    }
}
