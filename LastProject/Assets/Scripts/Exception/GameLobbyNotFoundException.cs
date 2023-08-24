public class GameLobbyNotFoundException : InGameException
{
    public GameLobbyNotFoundException() : base("You should join any lobby first before joining or creating room") { }
}
