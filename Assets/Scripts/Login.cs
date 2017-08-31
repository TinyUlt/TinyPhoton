
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Hive.Operations;
using ExitGames.Client.Photon;
using Photon.LoadBalancing.Events;



public class Login : MonoBehaviour, IPanelManager, IGameClientPeer
{

    PanelManager panelManager;
    GameClientPeer gameClient;

    public string IpAddress = "192.168.1.9:4530";
    public InputField InputField_Account;
    public InputField InputField_Password;

    public void SetPanelManager(PanelManager instance)
    {
        panelManager = instance;
    }
    public void OnPanelOpenMessage(object message)
    {
        gameClient.ConnectToServer(IpAddress);
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
        gameClient.RegisterEventCode(EventCode.AppStats, OnEvent);
        gameClient.RegisterConnectSuccessCallback(ConnectSuccess);
    }
    private void OnDisable()
    {
        if (gameClient == null)
        {
            return;
        }
        gameClient.UnRegisterOperationResponse(OperationCode.Authenticate, OnOperationResponse);
        gameClient.UnRegisterEventCode(EventCode.AppStats, OnEvent);
        gameClient.UnRegisterConnectSuccessCallback(ConnectSuccess);
    }
   
    void ConnectSuccess()
    {
        
    }
    public void OnButtonLogin()
    {
        Debug.Log("请求用户验证");
        OperationRequest request = new OperationRequest();
        request.OperationCode = (byte)OperationCode.Authenticate;
        request.Parameters = new Dictionary<byte, object>();
        request.Parameters.Add((byte)ParameterKey.UserAccount, InputField_Account.text);
        request.Parameters.Add((byte)ParameterKey.UserPassword, InputField_Password.text);
        gameClient.SendMessage(request);
    }
    public void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case (byte)EventCode.AppStats:
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
                    Debug.Log(operationResponse.DebugMessage);
                    if (operationResponse.ReturnCode == 0)
                    {
                        //登陆成功
                        PlayerPrefs.SetString("account", InputField_Account.text);
                        PlayerPrefs.SetString("password", InputField_Password.text);
                        panelManager.OpenPanel("Panel_Lobby");
                    }
                    else
                    {
                        //登陆失败
                    }
                    
                    break;
                }
        }
    }

   
}
