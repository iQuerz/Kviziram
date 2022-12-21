using StackExchange.Redis;

public static class KviziramConfig
{
    public static ConfigurationOptions redis = new ConfigurationOptions
    {
        EndPoints = { "localhost:49154" },
        ClientName = "default",
        Password = "redispw"
    };

    public const string NJ_ADDRESS = "http://localhost:7474";
    public const string NJ_USER = "neo4j";
    public const string NJ_PASS = "neo4jneo4j";

}