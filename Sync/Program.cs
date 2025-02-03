using Sync.Services;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Xml;

namespace Sync
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var config = LoadConfiguration();
                var apiKey = config.AppSettings.Settings["apiKey"].Value;
                string connectionString = config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString;

                await Sync(apiKey, connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task Sync(string apiKey, string connectionString)
        {
            var configuration = new Models.Configuration(apiKey);
            var virtuousService = new VirtuousService(configuration);
            var dapperService = new DapperService(connectionString);

            var skip = 0;
            var take = 100;
            var maxContacts = 1000;
            var hasMore = true;

            do
            {
                try
                {
                    var contacts = await virtuousService.GetContactsAsync(skip, take);
                    skip += take;
                    await dapperService.InsertContacts(contacts.List);
                    hasMore = skip > maxContacts;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            while (!hasMore);
        }

        private static Configuration LoadConfiguration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var debugConfigPath = AppDomain.CurrentDomain.BaseDirectory + "App.Debug.config";

            if (System.IO.File.Exists(debugConfigPath))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(debugConfigPath);

                foreach (XmlElement element in xmlDoc.DocumentElement)
                {
                    if (element.Name == "appSettings")
                    {
                        foreach (XmlElement addElement in element)
                        {
                            var key = addElement.GetAttribute("key");
                            var value = addElement.GetAttribute("value");
                            config.AppSettings.Settings[key].Value = value;
                        }
                    }
                    else if (element.Name == "connectionStrings")
                    {
                        foreach (XmlElement addElement in element)
                        {
                            var name = addElement.GetAttribute("name");
                            var connectionString = addElement.GetAttribute("connectionString");
                            config.ConnectionStrings.ConnectionStrings[name].ConnectionString = connectionString;
                        }
                    }
                }
            }

            return config;
        }
    }
}
