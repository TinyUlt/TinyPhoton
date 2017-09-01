using System.Collections.Generic;
using UnityEngine;
using Photon.Hive.Operations;
using ExitGames.Client.Photon;
using Photon.LoadBalancing.Events;
using UnityEngine.UI;
public class Lobby : PeerPanel
{
    public GameObject RoomInfoPanelForCopy;
    public InputField InputField_CreateRoom;
  
    string RoomId;
  
    override public void OnPanelOpenMessage(object message)
    {
        RoomInfoPanelForCopy.SetActive(false);
    }
   
    private void OnEnable()
    {
        if(gameClient == null)
        {
            return;
        }
        gameClient.RegisterOperationResponse(OperationCode.JoinLobby, OnOperationResponse);
        gameClient.RegisterOperationResponse(OperationCode.CreateGame, OnOperationResponse);
        gameClient.RegisterOperationResponse(OperationCode.JoinGame, OnOperationResponse);
        gameClient.RegisterEventCode(EventCode.AppStats, OnEvent);
        gameClient.RegisterEventCode(EventCode.GameList, OnEvent);
    }
    private void OnDisable()
    {
        if (gameClient == null)
        {
            return;
        }
        gameClient.UnRegisterOperationResponse(OperationCode.JoinLobby, OnOperationResponse);
        gameClient.UnRegisterOperationResponse(OperationCode.CreateGame, OnOperationResponse);
        gameClient.UnRegisterOperationResponse(OperationCode.JoinGame, OnOperationResponse);
        gameClient.RegisterEventCode(EventCode.AppStats, OnEvent);
        gameClient.UnRegisterEventCode(EventCode.GameList, OnEvent);
    }

    public void OnJoinLobby()
    {
        Debug.Log("请求加入大厅");
        OperationRequest request = new OperationRequest();
        request.OperationCode = (byte)OperationCode.JoinLobby;
        request.Parameters = new Dictionary<byte, object>();
        request.Parameters.Add((byte)ParameterKey.LobbyName, "Hall");
        gameClient.SendMessage(request);
    }
    public void OnCreateRoom()
    {
        Debug.Log("请求创建房间");
        OperationRequest request = new OperationRequest();
        request.OperationCode = (byte)OperationCode.CreateGame;
        request.Parameters = new Dictionary<byte, object>();
        request.Parameters.Add((byte)ParameterKey.GameId, InputField_CreateRoom.text);
        gameClient.SendMessage(request);
        RoomId = InputField_CreateRoom.text;
    }
    public void OnJoinRoom(string roomId)
    {
        Debug.Log("请求加入房间");
        OperationRequest request = new OperationRequest();
        request.OperationCode = (byte)OperationCode.JoinGame;
        request.Parameters = new Dictionary<byte, object>();
        request.Parameters.Add((byte)ParameterKey.GameId, roomId);
        gameClient.SendMessage(request);
        RoomId = roomId;
    }
    public void OnShowGameList(Hashtable table)
    {
        foreach(var room in table)
        {
            string roomname =(string) room.Key;
            Hashtable roominfo = room.Value as Hashtable;
            byte MaxPlayers = (byte)roominfo[(byte)GameParameter.MaxPlayers];
            bool IsOpen = (bool)roominfo[(byte)GameParameter.IsOpen];
            byte PlayerCount = (byte)roominfo[(byte)GameParameter.PlayerCount];

            GameObject go = GameObject.Instantiate(RoomInfoPanelForCopy) as GameObject;
            go.transform.FindChild("RoomId").GetComponent<Text>().text = roomname;
            go.transform.FindChild("RoomInfo").GetComponent<Text>().text = string.Format("MaxPlayers:{0},IsOpen:{1},PlayerCount:{2}",MaxPlayers, IsOpen, PlayerCount);
            go.transform.SetParent(RoomInfoPanelForCopy.transform.parent);
            go.SetActive(true);
            go.transform.GetComponent<Button>().onClick.AddListener(delegate () {
                this.OnJoinRoom(roomname);
            });
        }
    }
    public void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case (byte)EventCode.GameList:
                {
                    OnShowGameList((Hashtable)eventData.Parameters[(byte)ParameterKey.GameList]);
                    break;
                }
        }

    }
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch (operationResponse.OperationCode)
        {
            case (byte)OperationCode.CreateGame:
                {

                    Debug.Log(operationResponse.ToStringFull());
                    Hashtable table = new Hashtable();
                    table["ip"] = operationResponse.Parameters[(byte)ParameterKey.Address].ToString();
                    table["roomId"] = operationResponse.Parameters[(byte)ParameterKey.GameId].ToString(); ;
                    table["operation"] = OperationCode.CreateGame;
                    panelManager.OpenPanel("Panel_Room", table);
                    gameClient.Disconnect();
                    break;
                }
            case (byte)OperationCode.JoinGame:
                {
                    Debug.Log(operationResponse.ToStringFull());
                    Hashtable table = new Hashtable();
                    table["ip"] = operationResponse.Parameters[(byte)ParameterKey.Address].ToString();
                    table["roomId"] = RoomId;
                    table["operation"] = OperationCode.JoinGame;
                    panelManager.OpenPanel("Panel_Room", table);
                    gameClient.Disconnect();
                    break;
                }
        }
    }
}