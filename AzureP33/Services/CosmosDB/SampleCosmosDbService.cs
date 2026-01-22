using Microsoft.Azure.Cosmos;

namespace AzureP33.Services.CosmosDB
{
    public class SampleCosmosDbService : ICosmosDBService
    {
        private Container? _conteiner;
        private readonly IConfiguration _configuration;

        public SampleCosmosDbService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Container> GetConteinerAsync()
        {
            if (_conteiner == null)
            {
                IConfiguration sec = _configuration.GetSection("Azure")?.GetSection("CosmosDB");
                string connectionString = sec.GetValue<string>("ConnectionString") ?? throw new NullReferenceException("Configuration error: 'ConnectionString' is null");
                string databaseId = sec.GetValue<string>("DatabaseId") ?? throw new NullReferenceException("Configuration error: 'DatabaseId' is null");
                string conteinerId = sec.GetValue<string>("ConteinerId") ?? throw new NullReferenceException("Configuration error: 'ConnectionString' is null");

                CosmosClient client = new(
                    connectionString: connectionString
                );
                Database database = client.GetDatabase(databaseId);
                database = await database.ReadAsync();

                _conteiner = await database.GetContainer(conteinerId).ReadContainerAsync();
            }
            return _conteiner!;
        }
    }
}
