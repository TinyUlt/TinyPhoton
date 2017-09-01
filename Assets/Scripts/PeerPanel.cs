using System.Collections.Generic;
using UnityEngine;

using Photon.Hive.Operations;
using ExitGames.Client.Photon;
using Photon.LoadBalancing.Events;
public class PeerPanel : MonoBehaviour, IPanelManager, IGameClientPeer
{
    public PanelManager panelManager;
    public GameClientPeer gameClient;
    public void SetPanelManager(PanelManager instance)
    {
        panelManager = instance;
    }
    public void SetGameClientPeer(GameClientPeer instance)
    {
        gameClient = instance;
    }

    virtual public void OnPanelOpenMessage(object message)
    {
    }
}