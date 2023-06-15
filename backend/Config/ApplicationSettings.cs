using System.Text.Json.Serialization;

namespace Backend
{
    public class ApplicationSettings
    {
        public bool EnableAuthorization { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public DatabaseSettings DatabaseSettings { get; set; }
        public FileStorageSettings FileStorageSettings { get; set; }
    }

    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string TestConnectionString { get; set; }
        public string MainDatabaseName { get; set; }
        public string RefreshTokensCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string JellyfishCollectionName { get; set; }
    }

    public class FileStorageSettings
    {
        public string DebugFileStoragePath { get; set; }
        public string AzureConnectionString { get; set; }
        public string AzureContainerName { get; set; }
    }
}
