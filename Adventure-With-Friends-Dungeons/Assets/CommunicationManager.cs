using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CommunicationManager : MonoBehaviour
{
    PhotonView photon_view;
    public static CommunicationManager instance;
    private void Awake() {
        instance = this;
        photon_view = GetComponent<PhotonView>();
    }
    public void PostNotification(string message){
        byte[] m = System.Text.Encoding.UTF8.GetBytes(message);
        photon_view.RPC("RPC_PostNotification",RpcTarget.All,m);        
    }
    [PunRPC] void RPC_PostNotification(byte[] m_bytes) {
        string received_message =  System.Text.Encoding.UTF8.GetString(m_bytes);
        Debug.Log(received_message);
    }
}
