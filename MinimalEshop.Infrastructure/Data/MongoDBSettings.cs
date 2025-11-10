namespace MinimalEshop.Infrastructure.Data
    {
    public class MongoDBSettings
        {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string ProductsCollectionName { get; set; } = null!;
        }
    }

