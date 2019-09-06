using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using System;
using UnityEngine.UI;
[System.Serializable] public class PlayerData {
    public int character_id;
    public bool is_playing = false;
}
[System.Serializable]   public class PlayerManager : MonoBehaviourPun,IPunObservable
{
    //Data
    [SerializeField] public PlayerData data;

    //Accountability
    public bool is_setup = false;
    public int playerID;
    public int photon_viewID;
    public PhotonView photon_view;  
    public PlayerView player_view;
    public Camera player_camera;
    private void Start() {
        if(!photonView.IsMine) 
            return;
        StartCoroutine(WaitSetup(1));
        player_view.FadeOverlay(1,1000);
    }  

    private void Update() {
        if(!is_setup)
            return;
        
        if(!photon_view.IsMine)
            return;
    }
    public void InputNext(){
        Debug.Log("Input next");
        if(!GameManager.instance.is_adventure_started)
            return;
        StartCoroutine(InputNextRoutine());
    }
    IEnumerator InputNextRoutine() {
        if(GameManager.instance.is_migrating_host)
            yield break;
        GameManager.instance.is_migrating_host = true;
        GameManager.instance.SetMasterClient(PhotonNetwork.LocalPlayer.UserId);
        while(GameManager.instance.is_migrating_host)
            yield return null;
        GameManager.instance.AdventureNext();
        yield break;
    }

    //START - THE BEGGINING OF THE ADVENTURE
    public void InputStart(){
        Debug.Log("Input start");
        if(GameManager.instance.is_adventure_started)
            return;
        StartCoroutine(InputStartRoutine());
    }
    IEnumerator InputStartRoutine() {
        if(GameManager.instance.is_migrating_host)
            yield break;
        GameManager.instance.is_migrating_host = true;
        GameManager.instance.SetMasterClient(PhotonNetwork.LocalPlayer.UserId);
        while(GameManager.instance.is_migrating_host)
            yield return null;
        GameManager.instance.AdventureStart();
        yield break;
    }
    

    //SETUP - BEFORE THE ADVENTURE GET STARTED
    IEnumerator WaitSetup(float seconds){
        yield return new WaitForSeconds(seconds);
        if(!GameManager.instance.is_in_event)
            player_view.FadeOverlay(0,2);
        Setup();
        yield break;
    }
    
    void Setup() {
        if(!photon_view.IsMine)
            return;    
        photon_view.RPC("RPC_PlayerSetup",RpcTarget.All);
        PhotonView[] p_list = GameManager.instance.listOfPlayersPlaying;
        foreach (PhotonView p in p_list)
            p.GetComponent<PlayerManager>().RPC_PlayerSetup();
    }
    [PunRPC]public void RPC_PlayerSetup() {
        if(is_setup)
            return;    
        //Here is where you setup the player
        if(!photon_view.IsMine){
            //PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues((UnityEngine.Random.Range(99,99999)).ToString());
            player_camera.gameObject.SetActive(false);
        } else {
            data.is_playing = false;
            player_view.ToggleCharacterSelection(1);
            player_camera.gameObject.SetActive(true);
        }
        is_setup = true;
        return;
    }
    [PunRPC] public void RPC_DisableStartInput() {
        player_view.ToggleStartInput(false);
    }
    [PunRPC] public void RPC_ToggleNextInput(byte[] on_bytes) {
        if(!data.is_playing || !photonView.IsMine)
            return;
        player_view.ToggleNextInput(BitConverter.ToBoolean(on_bytes,0));
    }
    public void ChooseCharacter(int character_id){
        photon_view.RPC("RPC_ChooseCharacter",RpcTarget.AllBuffered,BitConverter.GetBytes(character_id));
    }
    [PunRPC]void RPC_ChooseCharacter(byte[] id_byte){
        StartCoroutine(ChooseCharacterRoutine(id_byte));
    }
    IEnumerator ChooseCharacterRoutine(byte[] id_byte) {
        data.character_id = BitConverter.ToInt32(id_byte,0);
        while(GameManager.instance.is_in_event)
            yield return null;
        player_view.FadeOverlay(0,2);
        player_view.ToggleAvatar(data.character_id);
        data.is_playing = true;
        if(!photon_view.IsMine)
            yield break;

        if(!GameManager.instance.is_adventure_started)
            player_view.ToggleStartInput(true);
        yield break;
    }
    public void UpdateGUI() {
        Debug.Log("Updated GUI");
        transform.position = new Vector3(playerID,0,0);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){}   
}
