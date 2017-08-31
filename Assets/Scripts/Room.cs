
using System.Collections.Generic;
using UnityEngine;

using Photon.Hive.Operations;
using ExitGames.Client.Photon;
using Photon.LoadBalancing.Events;
public class Room : MonoBehaviour, IPanelManager, IGameClientPeer
{
    PanelManager panelManager;
    GameClientPeer gameClient;
    string RoomId;
    OperationCode Operation;
    public void SetPanelManager(PanelManager instance)
    {
        panelManager = instance;
    }
    public void OnPanelOpenMessage(object message)
    {
        Hashtable table = message as Hashtable;
        string ip = table["ip"] as string;
        RoomId = table["roomId"] as string;
        Operation =(OperationCode) table["operation"];
        gameClient.ConnectToServer(ip);
    }
    public void SetGameClientPeer(GameClientPeer instance)
    {
        gameClient = instance;
    }
    private void OnEnable()
    {
        if (gameClient == null)
        {
            return;
        }
        gameClient.RegisterOperationResponse(OperationCode.Authenticate, OnOperationResponse);
        gameClient.RegisterOperationResponse(OperationCode.CreateGame, OnOperationResponse);
        gameClient.RegisterOperationResponse(OperationCode.JoinGame, OnOperationResponse);
        gameClient.RegisterOperationResponse(OperationCode.Join, OnOperationResponse);
        gameClient.RegisterEventCode(EventCode.Join, OnEvent);
        gameClient.RegisterConnectSuccessCallback(ConnectSuccess);
    }
    private void OnDisable()
    {
        if (gameClient == null)
        {
            return;
        }
        gameClient.UnRegisterOperationResponse(OperationCode.Authenticate, OnOperationResponse);
        gameClient.UnRegisterOperationResponse(OperationCode.CreateGame, OnOperationResponse);
        gameClient.UnRegisterOperationResponse(OperationCode.JoinGame, OnOperationResponse);
        gameClient.UnRegisterOperationResponse(OperationCode.Join, OnOperationResponse);
        gameClient.UnRegisterEventCode(EventCode.Join, OnEvent);
        gameClient.UnRegisterConnectSuccessCallback(ConnectSuccess);
    }
    
    void ConnectSuccess()
    {
        string account = PlayerPrefs.GetString("account", "");
        string password = PlayerPrefs.GetString("password", "");
        Debug.Log("请求用户认证");
        OperationRequest request = new OperationRequest();
        request.OperationCode = (byte)OperationCode.Authenticate;
        request.Parameters = new Dictionary<byte, object>();
        request.Parameters.Add((byte)ParameterKey.UserAccount, account);
        request.Parameters.Add((byte)ParameterKey.UserPassword, password);
        gameClient.SendMessage(request);
    }
    public void OnCreatRoom(string roomId)
    {
        Debug.Log("请求创建房间");
        OperationRequest request = new OperationRequest();
        request.OperationCode = (byte)OperationCode.CreateGame;
        request.Parameters = new Dictionary<byte, object>();
        request.Parameters.Add((byte)ParameterKey.GameId, roomId);
        gameClient.SendMessage(request);
    }
    public void OnJoinRoom(string roomId)
    {
        Debug.Log("请求加入房间");
        OperationRequest request = new OperationRequest();
        request.OperationCode = (byte)OperationCode.JoinGame;
        request.Parameters = new Dictionary<byte, object>();
        request.Parameters.Add((byte)ParameterKey.GameId, roomId);
        gameClient.SendMessage(request);
    }
    public void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case (byte)EventCode.Join:
                {
                    break;
                }
        }

    }
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch (operationResponse.OperationCode)
        {
            case (byte)OperationCode.Authenticate:
                {
                    if (operationResponse.ReturnCode == 0)
                    {
                        Debug.Log(operationResponse.DebugMessage);
                        //登陆成功
                        if (Operation == OperationCode.CreateGame)
                        {
                            OnCreatRoom(RoomId);

                        }else if(Operation == OperationCode.JoinGame)
                        {
                            OnJoinRoom(RoomId);
                        }
                        
                    }
                    else
                    {
                        //登陆失败
                    }
                    
                    break;
                }
            case (byte)OperationCode.CreateGame:
                {

                    Debug.Log(operationResponse.ToStringFull());
                    
                    break;
                }
            case (byte)OperationCode.Join:
            case (byte)OperationCode.JoinGame:
                {

                    Debug.Log(operationResponse.ToStringFull());

                    break;
                }
        }
    }
}
