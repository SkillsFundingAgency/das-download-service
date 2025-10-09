using System;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;

namespace SFA.DAS.DownloadService.Settings
{
    public static class ConfigurationService
    {
        public static async Task<IWebConfiguration> GetConfig(string environment, string storageConnectionString, string version, string serviceName)
        {
            ArgumentNullException.ThrowIfNull(environment);
            ArgumentNullException.ThrowIfNull(storageConnectionString);

            var tableClient = new TableClient(storageConnectionString, "Configuration");

            try
            {
                var result = await tableClient.GetEntityAsync<TableEntity>(
                    partitionKey: environment,
                    rowKey: $"{serviceName}_{version}"
                );

                string data = result.Value.GetString("Data");
                var webConfig = JsonConvert.DeserializeObject<WebConfiguration>(data);

                return webConfig;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                throw new RequestFailedException("Settings not found in table storage.");
            }
            catch (Exception ex)
            {
                throw new RequestFailedException("Could not connect to Storage to retrieve settings.", ex);
            }
        }
    }
}
