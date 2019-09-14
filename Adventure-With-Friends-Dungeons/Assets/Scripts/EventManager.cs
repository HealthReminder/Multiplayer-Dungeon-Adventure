using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class EventManager : MonoBehaviour
{
    public int current_event_id = 0;
    [SerializeField]public AdventureEvent current_event;
    public PhotonView photon_view;
    BackgroundView bv;
    [Header("Player Information")]
    [SerializeField] protected int current_place = 0;
    [SerializeField] protected int current_enemy = 0;
    private void Start() {
        bv = BackgroundView.instance;
    }

    public void NewEvent() {
        GameManager.instance.is_in_event = true;
        photon_view.RPC("RPC_NewEvent",RpcTarget.All, BitConverter.GetBytes(current_event_id),BitConverter.GetBytes(UnityEngine.Random.Range(0f,1f)));
    }
    [PunRPC] void RPC_NewEvent (byte[] id_byte, byte[] seed_byte) {
        int event_id = BitConverter.ToInt32(id_byte,0);
        float seed = BitConverter.ToSingle(seed_byte,0);
        Debug.Log("Encounter of id: "+event_id);
        if (current_event_id != event_id)
            return;
        current_event_id++;
        GameManager.instance.is_in_event = true;
        current_event.Initiate();

        StartCoroutine(EventRoutine());
    }

    IEnumerator EventRoutine() {
        //Do enemies first
        //For each place
        //Walk for x/2 seconds 
        //StopAt() place
        //yield return bv.ToggleMovementRoutine(0.5f,1);
        //yield return new WaitForSeconds(2);
        //yield return bv.ToggleMovementRoutine(0,1);
        for (int i = 0; i < current_event.places.Length ; i++)
        {
            current_place = i;
            yield return bv.ToggleMovementRoutine(0.5f,1);
            yield return new WaitForSeconds(1*current_event.places[i].distance);
            yield return bv.ToggleMovementRoutine(0,1);
            yield return bv.StopAtPlaceRoutine(current_event.places[i].sprite,0.1f);
            yield return new WaitForSeconds(3);
            yield return bv.LeavePlaceRoutine(0.05f);
        }
        GameManager.instance.TogglePlayersCombat(true);
        yield return new WaitForSeconds(10);
        GameManager.instance.TogglePlayersCombat(false);
        if(PhotonNetwork.IsMasterClient)
            EndEncounter();
        else RPC_EndEncounter();
        yield break;
    }  
    
    void EndEncounter() {
        photon_view.RPC("RPC_EndEncounter",RpcTarget.All);
    }

    [PunRPC] void RPC_EndEncounter() {
        GameManager.instance.is_in_event = false;
        StartCoroutine(BackgroundView.instance.ToggleMovementRoutine(0,1));

    }
}
