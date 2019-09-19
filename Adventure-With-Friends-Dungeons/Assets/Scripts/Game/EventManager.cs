using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

[System.Serializable]   public class EventData {
    public int current_event_id;
    public bool is_in_event = false;
}
public class EventManager : MonoBehaviour
{
    public EventData data;
    public GameObject[] adventure_events_available;
    public Transform adventure_event_container;
    [SerializeField]GameObject current_event_object;
    [SerializeField]AdventureEvent current_event_adventure;
    [HideInInspector] public PhotonView photon_view;
    BackgroundView bv;
    [Header("Player Information")]
    [SerializeField] protected int current_place = 0;
    [SerializeField] protected int current_enemy = 0;
    private void Start() {
        bv = BackgroundView.instance;
    }

    public void NewEvent() {
        data.is_in_event = true;
        photon_view.RPC("RPC_NewEvent",RpcTarget.All, BitConverter.GetBytes(data.current_event_id),BitConverter.GetBytes(UnityEngine.Random.Range(0f,1f)));
    }
    [PunRPC] void RPC_NewEvent (byte[] id_byte, byte[] seed_byte) {
        int event_id = BitConverter.ToInt32(id_byte,0);
        float seed = BitConverter.ToSingle(seed_byte,0);
        Debug.Log("Encounter of id: "+event_id);
        if (data.current_event_id != event_id)
            return;
        data.current_event_id++;
        data.is_in_event = true;
        current_event_object = Instantiate(adventure_events_available[0],transform.position,Quaternion.identity);
        current_event_object.transform.parent = adventure_event_container;
        current_event_object.transform.localPosition = new Vector3(0,0,0);
        current_event_adventure = current_event_object.GetComponent<AdventureEvent>();
        current_event_adventure.Initiate();

        StartCoroutine(EventRoutine(seed));
    }

    IEnumerator EventRoutine(float enemies_seed) {
        //Do enemies first
        //For each place
        //Walk for x/2 seconds 
        //StopAt() place
        //yield return bv.ToggleMovementRoutine(0.5f,1);
        //yield return new WaitForSeconds(2);
        //yield return bv.ToggleMovementRoutine(0,1);
        for (int i = 0; i < current_event_adventure.stops.Length ; i++)
        {
            current_place = i;
            Stop current_stop = current_event_adventure.stops[i];
            yield return bv.ToggleMovementRoutine(0.5f,1);
            yield return new WaitForSeconds(1*current_stop.distance);
            yield return bv.ToggleMovementRoutine(0,1);
            if(current_stop.enemies != null)
                if(current_stop.enemies.Length > 0)
                    EnemyManager.instance.UpdateState(current_stop.enemies,true);
            yield return current_event_adventure.ToggleStopRoutine(current_event_adventure.stops[i],1,true);
            yield return new WaitForSeconds(3);
            yield return current_event_adventure.ToggleStopRoutine(current_event_adventure.stops[i],1,false);
            EnemyManager.instance.UpdateState(current_stop.enemies,false);
        }
        //GameManager.instance.TogglePlayersCombat(true);
        //yield return new WaitForSeconds(10);
        //GameManager.instance.TogglePlayersCombat(false);
        if(PhotonNetwork.IsMasterClient)
            EndEncounter();
        else RPC_EndEncounter();
        yield break;
    }  
    
    void EndEncounter() {
        photon_view.RPC("RPC_EndEncounter",RpcTarget.All);
    }

    [PunRPC] void RPC_EndEncounter() {
        data.is_in_event = false;
        StartCoroutine(BackgroundView.instance.ToggleMovementRoutine(0,1));

    }
    public static EventManager instance;
    private void Awake() {
        instance = this;
    }
}
