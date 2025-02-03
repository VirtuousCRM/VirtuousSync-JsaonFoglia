using CsvHelper;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Sync
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var apiKey = config.GetSection("apiKey").Value;

            Sync(apiKey).GetAwaiter().GetResult();
        }

        private static async Task Sync(string apiKey)
        {
            if (apiKey == null) return;

            var configuration = new Configuration(apiKey);
            var virtuousService = new VirtuousService(configuration);

            var skip = 0;
            var take = 100;
            var maxContacts = 1000;
            var hasMore = true;

            using (var writer = new StreamWriter($"Contacts_{DateTime.Now:MM_dd_yyyy}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                do
                {
                    var contacts = await virtuousService.GetContactsAsync(skip, take);
                    skip += take;
                    csv.WriteRecords(contacts.List);
                    hasMore = skip > maxContacts;
                }
                while (!hasMore);
            }
        }
    }
}
