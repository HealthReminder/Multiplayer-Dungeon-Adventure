using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EventManager : MonoBehaviour
{
    public PhotonView photon_view;
    public void NewEnemyEncounter() {
        photon_view.RPC("RPC_NewEnemyEncounter",RpcTarget.All);
    }
    [PunRPC] void RPC_NewEnemyEncounter () {
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
    }
    IEnumerator SetBackgroundMovement(float target_speed,float step) {
        yield return ScenaryView.instance.ToggleMovementRoutine(target_speed,step);
        yield break;
    }
}
