
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Hive.Operations;
using ExitGames.Client.Photon;
using Photon.LoadBalancing.Events;
public class Room : PeerPanel
{

    string RoomId;
    OperationCode Operation;
    int Actornr;

    byte MaxPlayers;
    bool IsVisible;
    bool IsOpen;
    int MasterClientId;
    int ActorNr;
    override public void OnPanelOpenMessage(object message)
    {
        Hashtable table = message as Hashtable;
        string ip = table["ip"] as string;
        RoomId = table["roomId"] as string;
        Operation =(OperationCode) table["operation"];
        gameClient.ConnectToServer(ip);
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
        gameClient.RegisterEventCode(EventCode.Leave, OnEvent);
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
        gameClient.UnRegisterEventCode(EventCode.Leave, OnEvent);
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
    void SetGameProperty(Hashtable table, int actornr)
    {
        MaxPlayers = (byte)table[(byte)GameParameter.MaxPlayers];
        IsVisible = (bool)table[(byte)GameParameter.IsVisible];
        IsOpen = (bool)table[(byte)GameParameter.IsOpen];
        MasterClientId = (int)table[(byte)GameParameter.MasterClientId];
        ActorNr = actornr;
        ShowGameProperty();
    }
    void ShowGameProperty()
    {
        transform.FindChild("Text").GetComponent<Text>().text = string.Format("MaxPlayers:{0}\nIsVisible:{1}\nIsOpen:{2}\nMasterClientId:{3}\nActorNr:{4}\n", MaxPlayers, IsVisible, IsOpen, MasterClientId, ActorNr);
        // var PlayerCount = (byte)table[(byte)GameParameter.PlayerCount];
    }
    void UserJoin(Dictionary<byte, object> Parameters, bool self = false)
    {
        var actornr = (int)Parameters[(byte)ParameterKey.ActorNr];
        var actors = (int[])Parameters[(byte)ParameterKey.Actors];

        if (self)
        {
            SetGameProperty((Hashtable)Parameters[(byte)ParameterKey.GameProperties], actornr);
        }
      

        string actorlist = "";
        foreach(var actor in actors)
        {
            actorlist += actor.ToString() + ",";
        }
        transform.FindChild("Actor").GetComponent<Text>().text = string.Format("ActorJoin:{0}\n\n\nActorList:{1}", actornr, actorlist);
       
    }
    void UserLeave(Dictionary<byte, object> Parameters)
    {
        var actornr = (int)Parameters[(byte)ParameterKey.ActorNr];
        var actors = (int[])Parameters[(byte)ParameterKey.Actors];
        if (Parameters.ContainsKey((byte)ParameterKey.MasterClientId))
        {
            MasterClientId = (int)Parameters[(byte)ParameterKey.MasterClientId];
            ShowGameProperty();
        }
       
        string actorlist = "";
        foreach (var actor in actors)
        {
            actorlist += actor.ToString() + ",";
        }
        transform.FindChild("Actor").GetComponent<Text>().text = string.Format("ActorLeave:{0}\n\n\nActorList:{1}", actornr, actorlist);

        
    }
    public void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case (byte)EventCode.Join:
                {
                    UserJoin(eventData.Parameters);
                    break;
                }
            case (byte)EventCode.Leave:
                {
                    UserLeave(eventData.Parameters);
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
                    UserJoin(operationResponse.Parameters, true);

                    break;
                }
            case (byte)OperationCode.Join:
            case (byte)OperationCode.JoinGame:
                {
                    UserJoin(operationResponse.Parameters, true);

                    break;
                }
        }
    }
}
