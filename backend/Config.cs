using StackExchange.Redis;

namespace backend;

internal static class KviziramConfig
{
    public static ConfigurationOptions redis = new ConfigurationOptions
    {
        EndPoints = { "localhost:49154" },
        ClientName = "default",
        Password = "redispw"
    };

    public static string NJ_ADDRESS = "htpps://";
    public static string NJ_USER = "htpps://";
    public static string NJ_PASS = "htpps://";

}