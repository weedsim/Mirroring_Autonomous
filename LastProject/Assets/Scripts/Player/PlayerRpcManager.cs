using Fusion;
using UnityEngine;

public class PlayerRpcManager : SimulationBehaviour
{

    [Rpc]
    public static void RPC_RequestPlayerInfos(NetworkRunner runner, [RpcTarget] PlayerRef target, PlayerRef pr, string playerId)
    {
        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.Nickname = playerId;
        playerInfo.PlayerRef = pr;
        playerInfo.CharacterClassId = CharacterClassManager.Instance.GetDefaultCharacterClassId();

        PlayerManager.Instance.Notify_WhoAmI(playerInfo);

        Debug.Log("SERVER :: PLAYER Identifier : " + pr);
        Debug.Log("SERVER :: PLAYER DATA SEND TO " + playerId);

        PlayerInfoDTOUnit[] packed = PlayerManager.Instance.Pack();
        RPC_ResponsePlayerInfos(runner, pr, packed);
        RPC_AnnouncePlayerEntered(runner, PlayerManager.Instance.Convert(playerInfo));
    }

    [Rpc]
    public static void RPC_ResponsePlayerInfos(NetworkRunner runner, [RpcTarget] PlayerRef target, PlayerInfoDTOUnit[] instances)
    {
        PlayerManager.Instance.SetLocalPlayerRef(target);
        PlayerManager.Instance.Unpack(instances);
        Debug.Log("CLIENT :: PLAYER DATA RECEIVED");
    }

    [Rpc]
    public static void RPC_AnnouncePlayerEntered(NetworkRunner runner, PlayerInfoDTOUnit instance)
    {
        Debug.Log("CLIENT :: PLAYER " + instance.PlayerId + " JOINED DATA RECEIVED");
        PlayerManager.Instance.RegisterPlayer(PlayerManager.Instance.Convert(instance));
    }

    [Rpc]
    public static void RPC_AnnouncePlayerExited(NetworkRunner runner, PlayerRef pr)
    {
        Debug.Log("CLIENT :: PLAYER " + PlayerManager.Instance.GetPlayerInfo(pr).Nickname + " LEFT DATA RECEIVED");
        PlayerManager.Instance.ExitPlayer(pr);
    }

    [Rpc]
    public static void RPC_RequestChangeCharacgterClass(NetworkRunner runner, [RpcTarget] PlayerRef target, PlayerRef player, int characterClassId)
    {
        Debug.Log("SERVER :: CHANGE CHARACTER CLASS OF " + PlayerManager.Instance.GetPlayerInfo(player).Nickname + " Class To : " + CharacterClassManager.Instance.GetCharacterClassInfo(characterClassId).CharacterClassName);
        PlayerManager.Instance.ChangeCharacterClass(player, characterClassId);
        RPC_AnnounceChangeCharacgterClass(runner, player, characterClassId);
    }

    [Rpc]
    public static void RPC_AnnounceChangeCharacgterClass(NetworkRunner runner, PlayerRef player, int characterClassId)
    {
        Debug.Log("CLIENT :: CHANGE CHARACTER CLASS OF " + PlayerManager.Instance.GetPlayerInfo(player).Nickname + " Class To : " + CharacterClassManager.Instance.GetCharacterClassInfo(characterClassId).CharacterClassName);
        PlayerManager.Instance.ChangeCharacterClass(player, characterClassId);
    }


}
