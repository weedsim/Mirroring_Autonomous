public class SessionConfig
{
    public string SessionName { get; protected set; }
    public string Region { get; protected set; }
    public string Lobby { get; protected set; }
    public ushort Port { get; protected set; }
    public ushort PublicPort { get; protected set; }
    public string PublicIP { get; protected set; }

    protected SessionConfig() { }

    public static SessionConfig CreateDefaultLobbyConfig()
    {
        var config = new SessionConfig();
        config.SessionName = "Lobby";
        config.Lobby = "Lobby";
        config.Region = "kr";
        config.PublicPort = 5058;
        return config;
    }


    private static SessionConfig defaultLobbyConfig = CreateDefaultLobbyConfig();
    public static SessionConfig GetDefaultLobbyConfig() 
    {
        return defaultLobbyConfig;
    }
}
