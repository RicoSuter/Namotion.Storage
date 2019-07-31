using Namotion.Storage.Abstractions;
using Namotion.Storage.Azure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace Namotion.Storage.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var connectionString = "DefaultEndpointsProtocol=https;AccountName=namotionstoragetests;" +
                "AccountKey=uJ5+D+LrP+bSCLL0ZnEkt+IcMokRMUWbXwqeEUe/qWLRLJH7F/" +
                "RX93BZCiHk/L6dhRzudOeYVPMo2IiaS1whGA==;EndpointSuffix=core.windows.net";

            var storage = AzureBlobStorage.CreateFromConnectionString(connectionString);
            var container = storage.GetContainer("foo");

            // Check existing
            if (await container.ExistsAsync("abc2"))
            {
                var output2 = await container.ReadAsStringAsync("abc2");
                Console.WriteLine("Output: " + output2);
            }

            // Write & read string
            await container.WriteAsStringAsync("abc", "Hello world! " + DateTimeOffset.Now);
            var output = await container.ReadAsStringAsync("abc");
            Console.WriteLine("Output: " + output);

            var items = await container.ListAsync();

            // Write & read JSON
            var inputPerson = new
            {
                FirstName = "Rico " + DateTimeOffset.Now,
                LastName = "Suter"
            };

            await container.WriteAsJsonAsync("person", inputPerson);
            var outputPerson = await container.ReadAsJsonAsync<dynamic>("person");

            Console.WriteLine("Firstname: " + outputPerson.firstName);
            Console.WriteLine("Lastname: " + outputPerson.lastName);
        }
    }
}
