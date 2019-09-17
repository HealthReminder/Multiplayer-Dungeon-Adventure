using System;
using System.Collections;
using System.Collections.Generic;
using OdinSerializer;
using Photon.Pun;
using UnityEngine;

[System.Serializable] public class GameManager : MonoBehaviour {

    public PlayerManager[] listOfPlayersPlaying;
    public PhotonView photon_view;
    public EventManager event_manager;
    //Conditions for match to begin
    int min_players_max_players = 2;
    //Game state
    public bool is_adventure_started = false;

    public bool is_synchronizing_players = false;
    public bool is_migrating_host = false;

    public static GameManager instance;
    void Awake () {
        instance = this;
        photon_view = GetComponent<PhotonView> ();
    }
    private void Update () {
        //This will ensure that every player is in the game before starting the game loop
        //This is fed by the room controller
        if (!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < listOfPlayersPlaying.Length; i++)
            if (listOfPlayersPlaying[i] == null)
                OnPlayerLeftRoom ();

    }
    
#region Loop
    //MIDDLE - DURING THE ADVENTURE
    public void TogglePlayersCombat(bool is_on) {
        photon_view.RPC("RPC_TogglePlayersCombat",RpcTarget.All,BitConverter.GetBytes(is_on));
    }
    [PunRPC] public void RPC_TogglePlayersCombat(byte[] on_byte) {
        bool is_on = BitConverter.ToBoolean(on_byte,0);
        foreach (PlayerManager p in listOfPlayersPlaying)
            if(p.data.is_playing)
                if(p.photon_view.IsMine)
                    p.ToggleCombat(is_on);            
    }
    IEnumerator EnableNextRoutine() {
        yield return null;
        while(event_manager.data.is_in_event)
            yield return null;
        yield return new WaitForSeconds(1);
        for (int i = 0; i < listOfPlayersPlaying.Length; i++)
            if (listOfPlayersPlaying[i] != null)
                listOfPlayersPlaying[i].photon_view.RPC("RPC_ToggleNextInput",RpcTarget.All, BitConverter.GetBytes(true));
        yield break;
    }
    public void AdventureNext() {
        photon_view.RPC("RPC_AdventureNext",RpcTarget.AllViaServer);
    }
    [PunRPC] void RPC_AdventureNext() {
        Debug.Log("RPC_AdventureNext");
        if(!is_adventure_started)
            return;
        
        if(!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < listOfPlayersPlaying.Length; i++)
            if (listOfPlayersPlaying[i] != null)
                listOfPlayersPlaying[i].photon_view.RPC("RPC_ToggleNextInput",RpcTarget.All,BitConverter.GetBytes(false));
        event_manager.NewEvent();
        StartCoroutine(EnableNextRoutine());
    }
#endregion
#region Start

    //START - THE BEGINNING OF THE ADVENTURE
    public void AdventureStart() {
        photon_view.RPC("RPC_AdventureStart",RpcTarget.AllViaServer);
    }
    [PunRPC] void RPC_AdventureStart() {
        Debug.Log("RPC_AdventureStart");
        if(is_adventure_started)
            return;
        is_adventure_started = true;
        
        if(!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < listOfPlayersPlaying.Length; i++)
            if (listOfPlayersPlaying[i] != null)
                listOfPlayersPlaying[i].photon_view.RPC("RPC_DisableStartInput",RpcTarget.All);
        event_manager.NewEvent();
        StartCoroutine(EnableNextRoutine());
    }
#endregion
#region Host migration
    //SET MASTER CLIENT
    public void SetMasterClient(string user_id){
        photon_view.RPC("RPC_SetMasterClient",RpcTarget.AllViaServer,System.Text.Encoding.UTF8.GetBytes(user_id));
    }
    [PunRPC]void RPC_SetMasterClient(byte[] user_id_bytes) {
        is_migrating_host = true;
        string received_user =  System.Text.Encoding.UTF8.GetString(user_id_bytes);
        var p = PhotonNetwork.MasterClient;
        //CommunicationManager.instance.PostNotification("Received host: "+received_user);
        //CommunicationManager.instance.PostNotification("Current host: "+p.UserId);
        foreach (var i in PhotonNetwork.PlayerList)
            if(i.UserId == received_user)
                p = i;
        //CommunicationManager.instance.PostNotification("New host: "+p.UserId);
        PhotonNetwork.SetMasterClient(p);
        //CommunicationManager.instance.PostNotification("Set host: "+p.UserId);
        is_migrating_host = false;
        ChatManager.instance.AddEntry(GameManager.instance.listOfPlayersPlaying[int.Parse(PhotonNetwork.MasterClient.NickName)-1].data.player_name, " is leading the way!","#7d3c98","#884ea0", false);
    }
#endregion
#region Player in Room
    public void OnPlayerLeftRoom () {
        Debug.Log ("A player left the room.");
        if (!PhotonNetwork.IsMasterClient)
            return;

        List<PlayerManager> p = new List<PlayerManager>();

        for (int i = 0; i < listOfPlayersPlaying.Length; i++)
            p.Add(listOfPlayersPlaying[i]);

        for (int o = p.Count - 1; o >= 0; o--)
            if (p[o] == null){
                p.RemoveAt (o);
            }

        listOfPlayersPlaying = new PlayerManager[p.Count];
        for (int u = 0; u < p.Count; u++)
            listOfPlayersPlaying[u] = p[u];

        ChatManager.instance.AddEntry("Someone"," abandoned the adventure.","#154360","#1b4f72",false);

        SynchronizeAllPlayers(listOfPlayersPlaying);
    }
    [PunRPC] public void RPC_AddPlayer (byte[] viewBytes,byte[] name_bytes) {
        //This function is responsible for adding the player to the listOfPlayersPlaying 
        //So the update function can start the match when all the players have loaded the room properly
        int newPlayerIndex = -1;
        if (listOfPlayersPlaying == null || listOfPlayersPlaying.Length <= 0) {
            listOfPlayersPlaying = new PlayerManager[1];
            newPlayerIndex = 0;
        } else {
            PlayerManager[] newList = new PlayerManager[listOfPlayersPlaying.Length + 1];
            newPlayerIndex = listOfPlayersPlaying.Length;

            for (int i = 0; i < listOfPlayersPlaying.Length; i++) {
                newList[i] = listOfPlayersPlaying[i];
            }
            listOfPlayersPlaying = newList;
            //Debug.Log ("Created new array");
        }

        //Deserialize information to get the viewID so the player PhotonView can be found in the network
        //And added to the player list and also be setup
        int receivedPhotonViewID = BitConverter.ToInt32 (viewBytes, 0);
        string received_name = System.Text.Encoding.UTF8.GetString(name_bytes);

        //Debug.Log ("Player "+received_name+" with view ID of" + receivedPhotonViewID + " joined the room with ID of " + newPlayerIndex);
        ChatManager.instance.AddEntry(received_name, " joined the adventure!","#2471a3","#2e86c1",false);

        PhotonView playerView = PhotonNetwork.GetPhotonView (receivedPhotonViewID);
        Debug.Log ("Adding new player with index of " + newPlayerIndex + " to the list of size " + listOfPlayersPlaying.Length);
        listOfPlayersPlaying[newPlayerIndex] = playerView.GetComponent<PlayerManager>();
        listOfPlayersPlaying[newPlayerIndex].data = new PlayerData();
        listOfPlayersPlaying[newPlayerIndex].data.Reset(received_name,receivedPhotonViewID,newPlayerIndex);

        if (!PhotonNetwork.IsMasterClient)
            return;
        
        SynchronizeAllPlayers(listOfPlayersPlaying);
    }
#endregion
#region Synchronization
    public void SynchronizeAllPlayers(PlayerManager[] ps){
        for (int i = 0; i < ps.Length; i++){
            photon_view.RPC ("RPC_SynchronizePlayer", RpcTarget.All,
                BitConverter.GetBytes(ps[i].photon_view.ViewID),
                BitConverter.GetBytes(ps.Length),
                //GameMAnager
                BitConverter.GetBytes(is_adventure_started),
                //EventManager
                Serialization.instance.SerializeEventData(event_manager.data),
                //PlayerData
                Serialization.instance.SerializePlayerData(ps[i].data)
            ); 
        }
    }

    [PunRPC] public void RPC_SynchronizePlayer (byte[] viewIDBytes, byte[] listSizeBytes,byte[] is_started_byte,byte[] event_data_bytes, byte[] player_data_bytes) {
        Debug.Log ("RPC_SynchronizePlayer");
        is_synchronizing_players = true;
        

        int receivedViewId = BitConverter.ToInt32 (viewIDBytes, 0);
        PhotonView receivedPlayer = PhotonNetwork.GetPhotonView (receivedViewId);
        int receivedSize = BitConverter.ToInt32 (listSizeBytes, 0);

        
        
        bool started = BitConverter.ToBoolean(is_started_byte,0);

        
        is_adventure_started = started;

        PlayerManager received_player_manager = receivedPlayer.GetComponent<PlayerManager> ();
        received_player_manager.photon_viewID = receivedViewId;

        //EventData
        event_manager.data = Serialization.instance.DeserializeEventData(event_data_bytes);

        //PlayerData
        received_player_manager.data = Serialization.instance.DeserializePlayerData(player_data_bytes);

        //Update GUI
        received_player_manager.UpdatePosition();

        //If there is no list create it
        if (listOfPlayersPlaying == null || listOfPlayersPlaying.Length != receivedSize)
            listOfPlayersPlaying = new PlayerManager[receivedSize];

        listOfPlayersPlaying[received_player_manager.data.player_id] = receivedPlayer.GetComponent<PlayerManager>();

        is_synchronizing_players = false;
        Debug.Log ("Finished synchronizing player data");
    }
#endregion
}