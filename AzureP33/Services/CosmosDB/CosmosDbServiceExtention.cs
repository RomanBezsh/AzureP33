namespace AzureP33.Services.CosmosDB
{
    public static class CosmosDbServiceExtention
    {
        public static void AddCosmosDb(this IServiceCollection services)
        {
            services.AddSingleton<ICosmosDBService,SampleCosmosDbService>();
        }
    }
}
