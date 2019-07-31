using Namotion.Storage.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Namotion.Storage
{
    public static class NewtonsoftJsonBlobExtensions
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private static readonly JsonSerializer _serializer = JsonSerializer.Create(_serializerSettings);

        public static Task WriteAsJsonAsync<T>(this IBlobWriter writer, string path, T value, CancellationToken cancellationToken = default)
        {
            return writer.WriteAsJsonAsync(path, value, _serializer, cancellationToken);
        }

        public static async Task WriteAsJsonAsync<T>(this IBlobWriter writer, string path, T value, JsonSerializer serializer, CancellationToken cancellationToken = default)
        {
            using (var jsonWriter = new JsonTextWriter(new StreamWriter(await writer.OpenWriteAsync(path, cancellationToken).ConfigureAwait(false))))
            {
                serializer.Serialize(jsonWriter, value);
            }
        }

        public static Task<T> ReadAsJsonAsync<T>(this IBlobReader reader, string path, CancellationToken cancellationToken = default)
        {
            return reader.ReadAsJsonAsync<T>(path, _serializer, cancellationToken);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this IBlobReader reader, string path, JsonSerializer serializer, CancellationToken cancellationToken = default)
        {
            using (var jsonReader = new JsonTextReader(new StreamReader(await reader.OpenReadAsync(path, cancellationToken).ConfigureAwait(false))))
            {
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}
