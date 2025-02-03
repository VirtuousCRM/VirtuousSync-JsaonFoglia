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
            string connectionString = config.GetConnectionString("DefaultConnection");

            Sync(apiKey, connectionString).GetAwaiter().GetResult();
        }

        private static async Task Sync(string apiKey, string connectionString)
        {
            var configuration = new Configuration(apiKey);
            var virtuousService = new VirtuousService(configuration);
            var dapperService = new DapperService(connectionString);

            var skip = 0;
            var take = 100;
            var maxContacts = 1000;
            var hasMore = true;

            do
            {
                var contacts = await virtuousService.GetContactsAsync(skip, take);
                skip += take;
                await dapperService.InsertContacts(contacts.List);
                hasMore = skip > maxContacts;
            }
            while (!hasMore);
        }
    }
}
