using System;
using System.Collections;
using System.Collections.Generic;
using OdinSerializer;
using Photon.Pun;
using UnityEngine;

[System.Serializable] public class GameManager : MonoBehaviour {

    public PhotonView[] listOfPlayersPlaying;

    public PhotonView photon_view;
    //Conditions for match to begin
    int min_players_max_players = 2;

    //Game state
    public bool is_adventure_started = false;
    public bool is_synchronizing_players = false;

    public static GameManager instance;
    void Awake () {
        instance = this;
        photon_view = GetComponent<PhotonView> ();
    }

    //START - THE BEGINNING OF THE ADVENTURE
    public void StartGame() {
        //PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        //Debug.Log(PhotonNetwork.LocalPlayer.NickName + "  " + PhotonNetwork.MasterClient.NickName);
        photon_view.RPC("RPC_StartGame",RpcTarget.AllViaServer);
    }
    [PunRPC] void RPC_StartGame() {
        if(is_adventure_started)
            return;
        is_adventure_started = true;
        
        if(!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < listOfPlayersPlaying.Length; i++)
            if (listOfPlayersPlaying[i] != null)
                listOfPlayersPlaying[i].RPC("RPC_DisableStartInput",RpcTarget.All);
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
    public void OnPlayerLeftRoom () {
        Debug.Log ("A player left the room.");
        if (!PhotonNetwork.IsMasterClient)
            return;

        List<PhotonView> p = new List<PhotonView> ();

        for (int i = 0; i < listOfPlayersPlaying.Length; i++)
            p.Add (listOfPlayersPlaying[i]);

        for (int o = p.Count - 1; o >= 0; o--)
            if (p[o] == null)
                p.RemoveAt (o);

        listOfPlayersPlaying = new PhotonView[p.Count];
        for (int u = 0; u < p.Count; u++)
            listOfPlayersPlaying[u] = p[u];

        SynchronizeAllPlayers(listOfPlayersPlaying);
    }
    public void SynchronizeAllPlayers(PhotonView[] ps){
        for (int i = 0; i < ps.Length; i++){
            PlayerData d = ps[i].GetComponent<PlayerManager>().data;
            photon_view.RPC ("RPC_SynchronizePlayer", RpcTarget.All,
                BitConverter.GetBytes(ps[i].ViewID),
                BitConverter.GetBytes(i),
                BitConverter.GetBytes(ps.Length),
                //game
                BitConverter.GetBytes(is_adventure_started),
                //data
                BitConverter.GetBytes(d.character_id),
                BitConverter.GetBytes(d.is_playing)
            ); 
        }
    }

    [PunRPC] public void RPC_SynchronizePlayer (byte[] viewIDBytes, byte[] indexBytes, byte[] listSizeBytes,byte[] is_started_byte, byte[] data_character_id_byte, byte[] is_playing_byte) {
        Debug.Log ("RPC_SynchronizePlayer");
        is_synchronizing_players = true;
        

        int receivedViewId = BitConverter.ToInt32 (viewIDBytes, 0);
        PhotonView receivedPlayer = PhotonNetwork.GetPhotonView (receivedViewId);
        int receivedIndex = BitConverter.ToInt32 (indexBytes, 0);
        int receivedSize = BitConverter.ToInt32 (listSizeBytes, 0);
        bool is_playing = BitConverter.ToBoolean(is_playing_byte,0);
        bool is_started = BitConverter.ToBoolean(is_started_byte,0);

        is_adventure_started = is_started;

        PlayerManager received_manager = receivedPlayer.GetComponent<PlayerManager> ();
        received_manager.playerID = receivedIndex;
        received_manager.photon_viewID = receivedViewId;

        //data
        received_manager.data.character_id = BitConverter.ToInt32(data_character_id_byte,0);
        received_manager.data.is_playing = is_playing;

        //Update GUI
        received_manager.UpdateGUI();

        //If there is no list create it
        if (listOfPlayersPlaying == null || listOfPlayersPlaying.Length != receivedSize)
            listOfPlayersPlaying = new PhotonView[receivedSize];

        listOfPlayersPlaying[receivedIndex] = receivedPlayer;

        is_synchronizing_players = false;
        Debug.Log ("Finished synchronizing player data");
    }

    [PunRPC] public void RPC_AddPlayer (byte[] viewBytes) {
        //This function is responsible for adding the player to the listOfPlayersPlaying 
        //So the update function can start the match when all the players have loaded the room properly
        int newPlayerIndex = -1;
        if (listOfPlayersPlaying == null || listOfPlayersPlaying.Length <= 0) {
            listOfPlayersPlaying = new PhotonView[1];
            newPlayerIndex = 0;
        } else {
            PhotonView[] newList = new PhotonView[listOfPlayersPlaying.Length + 1];
            newPlayerIndex = listOfPlayersPlaying.Length;

            for (int i = 0; i < listOfPlayersPlaying.Length; i++) {
                newList[i] = listOfPlayersPlaying[i];
            }
            listOfPlayersPlaying = newList;
            Debug.Log ("Created new array");
        }

        //Deserialize information to get the viewID so the player PhotonView can be found in the network
        //And added to the player list and also be setup
        int receivedPhotonViewID = BitConverter.ToInt32 (viewBytes, 0);
        Debug.Log ("Player " + receivedPhotonViewID + " joined the room with ID of " + newPlayerIndex);

        PhotonView playerView = PhotonNetwork.GetPhotonView (receivedPhotonViewID);
        Debug.Log ("Adding new player with index of " + newPlayerIndex + " to the list of size " + listOfPlayersPlaying.Length);
        listOfPlayersPlaying[newPlayerIndex] = playerView;

        if (!PhotonNetwork.IsMasterClient)
            return;
        
        SynchronizeAllPlayers(listOfPlayersPlaying);
    }

}