using Microsoft.Azure.Cosmos;

namespace AzureP33.Services.CosmosDB
{
    public interface ICosmosDBService
    {
        Task<Container> GetConteinerAsync();
    }
}
