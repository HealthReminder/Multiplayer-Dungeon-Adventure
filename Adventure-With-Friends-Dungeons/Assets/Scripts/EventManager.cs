using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class EventManager : MonoBehaviour
{
    public int current_event = 0;
    public PhotonView photon_view;
    public void NewEnemyEncounter() {
        GameManager.instance.is_in_event = true;
        photon_view.RPC("RPC_NewEnemyEncounter",RpcTarget.All, BitConverter.GetBytes(current_event));
    }
    [PunRPC] void RPC_NewEnemyEncounter (byte[] id_byte) {
        int event_id = BitConverter.ToInt32(id_byte,0);
        Debug.Log("Encounter of id: "+event_id);
        if (current_event != event_id)
            return;
        current_event++;
        GameManager.instance.is_in_event = true;
        //if(!PhotonNetwork.IsMasterClient)
            //return;
        StartCoroutine(EnemyEncounterRoutine());
    }
    IEnumerator EnemyEncounterRoutine() {
        yield return SetBackgroundMovement(0.5f,1);
        yield return new WaitForSeconds(4);
        yield return SetBackgroundMovement(0,1);
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
