public class PlayerIsAlreadyJoinedRoomException : InGameException
{
    public PlayerIsAlreadyJoinedRoomException() : base("You should exit the room you belong to in order to join a room") { }
}
