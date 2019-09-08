using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
[System.Serializable]   public struct Event {
    public Enemy[] enemies;
}
public class EventManager : MonoBehaviour
{
    public int current_event_id = 0;
    [SerializeField]public Event current;
    public PhotonView photon_view;
    public void NewEnemyEncounter() {
        GameManager.instance.is_in_event = true;
        photon_view.RPC("RPC_NewEnemyEncounter",RpcTarget.All, BitConverter.GetBytes(current_event_id),BitConverter.GetBytes(UnityEngine.Random.Range(0f,1f)));
    }
    [PunRPC] void RPC_NewEnemyEncounter (byte[] id_byte, byte[] seed_byte) {
        int event_id = BitConverter.ToInt32(id_byte,0);
        float seed = BitConverter.ToSingle(seed_byte,0);
        Debug.Log("Encounter of id: "+event_id);
        if (current_event_id != event_id)
            return;
        current_event_id++;
        GameManager.instance.is_in_event = true;
        StartCoroutine(EnemyEncounterRoutine());
    }

    IEnumerator EnemyEncounterRoutine() {
        yield return SetBackgroundMovement(0.5f,1);
        yield return new WaitForSeconds(2);
        yield return SetBackgroundMovement(0,1);
        GameManager.instance.TogglePlayersCombat(true);
        yield return new WaitForSeconds(2);
        GameManager.instance.TogglePlayersCombat(false);
        if(PhotonNetwork.IsMasterClient)
            EndEnemyEncounter();
        else RPC_EndEnemyEncounter();
        yield break;
    }
    
    void EndEnemyEncounter() {
        photon_view.RPC("RPC_EndEnemyEncounter",RpcTarget.All);
    }

    [PunRPC] void RPC_EndEnemyEncounter() {
        GameManager.instance.is_in_event = false;
        StartCoroutine(SetBackgroundMovement(0,1));

    }
    IEnumerator SetBackgroundMovement(float target_speed,float step) {
        yield return BackgroundView.instance.ToggleMovementRoutine(target_speed,step);
        yield break;
    }
}
