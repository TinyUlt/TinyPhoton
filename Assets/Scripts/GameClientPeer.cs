
using System.Collections.Generic;
using UnityEngine;


using ExitGames.Client.Photon;
using Photon.Hive.Operations;
using Photon.LoadBalancing.Events;
using System;


public class GameClientPeer : MonoBehaviour , IPhotonPeerListener
{
    public string ServerName = "Master";
    
    public GameObject[] UsePeerPanelList;

    private PhotonPeer peer;
    
    private Dictionary<OperationCode, OpResCallback> OperationResponseContain;
    private Dictionary<EventCode, EventDataCallback> EventDataContain;
    private ConnectSuccessCallback connectCallback;
    public void RegisterConnectSuccessCallback(ConnectSuccessCallback Callback)
    {
        if(connectCallback == null)
        {
            connectCallback = Callback;
        }
        else
        {
            connectCallback += Callback;
        }
    }
    public void UnRegisterConnectSuccessCallback(ConnectSuccessCallback Callback)
    {
        if (connectCallback == null)
        {
            Debug.LogWarning("未注册连接成功回调:");
        }
        else
        {
            connectCallback -= Callback;
        }
    }
    public void RegisterOperationResponse(OperationCode code, OpResCallback callback)
    {
        if (OperationResponseContain.ContainsKey(code))
        {
            OperationResponseContain[code] += callback;
        }
        else
        {
            OperationResponseContain[code] = callback;
        }
    }
    public void UnRegisterOperationResponse(OperationCode code, OpResCallback callback)
    {
        if (OperationResponseContain.ContainsKey(code))
        {
            OperationResponseContain[code] -= callback;

            if(OperationResponseContain[code] == null)
            {
                OperationResponseContain.Remove(code);
            }
        }else
        {
            Debug.LogWarning("未注册消息:" + code.ToString());
        }
       
    }
    public void RegisterEventCode(EventCode code, EventDataCallback callback)
    {
        if (EventDataContain.ContainsKey(code))
        {
            EventDataContain[code] += callback;
        }
        else
        {
            EventDataContain[code] = callback;
        }
    }
    public void UnRegisterEventCode(EventCode code, EventDataCallback callback)
    {
        if (EventDataContain.ContainsKey(code))
        {
            EventDataContain[code] -= callback;

            if (EventDataContain[code] == null)
            {
                EventDataContain.Remove(code);
            }
        }
        else
        {
            Debug.LogWarning("未注册事件:" + code.ToString());
        }
    }
    private void Awake()
    {
        foreach (GameObject child in UsePeerPanelList)
        {
            var instance = child.GetComponent<IGameClientPeer>();
            if (instance != null)
            {
                instance.SetGameClientPeer(this);
            }
        }
        OperationResponseContain = new Dictionary<OperationCode, OpResCallback>();
        EventDataContain = new Dictionary<EventCode, EventDataCallback>();
    }
    void Start()
    {
        Debug.Log("start");
    }
    public void ConnectToServer(string address)
    {
        if (peer != null)
        {
            if(peer.PeerState!= PeerStateValue.Disconnected)
            {
                Disconnect();
            }
        }
        peer = new PhotonPeer(this, ConnectionProtocol.Tcp);
        peer.Connect(address, ServerName);
    }
 
    void Update()
    {
        if(peer != null)
        {
            peer.Service();
        }
       
    }

  
    public void SendMessage(OperationRequest request)
    {
        peer.OpCustom(request, true, 0, false);
    }
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    public void OnEvent(EventData eventData)
    {
        Debug.Log("触发了事件：" + eventData.ToStringFull());
        EventCode code = (EventCode)eventData.Code;
        if (EventDataContain.ContainsKey(code) && EventDataContain[code]!=null)
        {
            EventDataContain[code](eventData);
        }else
        {
            Debug.LogError("未注册事件:" + code.ToString());
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        Debug.Log("返回消息：" + operationResponse.ToStringFull());
        OperationCode code = (OperationCode)operationResponse.OperationCode;
        if (OperationResponseContain.ContainsKey(code))
        {
            OperationResponseContain[code](operationResponse);
        }
        else
        {
            Debug.LogError("未注册返回消息:" + code.ToString());
        }
    }
    void ExcuteMessage(OperationResponse response)
    {

    }
    public void OnStatusChanged(StatusCode statusCode)
    {
        switch (statusCode)
        {
            case StatusCode.Connect:

                Debug.Log("连接成功");

                if (connectCallback != null) {
                    connectCallback();
                }
                break;
            case StatusCode.Disconnect:
                Debug.Log("关闭连接");
                break;
            case StatusCode.ExceptionOnConnect:
                Debug.Log("连接异常");
                break;
        }
    }

    public void OnMessage(object messages)
    {
        throw new NotImplementedException();
    }
    public void Disconnect()
    {
        if(peer != null)
        {
            peer.Disconnect();
            peer = null;
        }
    }
    private void OnDestroy()
    {
        Disconnect();
    }
}
