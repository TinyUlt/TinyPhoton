

public interface IPanelManager
{
    void SetPanelManager(PanelManager instance);
    void OnPanelOpenMessage(object message);
}

public interface IGameClientPeer
{
    void SetGameClientPeer(GameClientPeer instance);
}