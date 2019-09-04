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
        if(!PhotonNetwork.IsMasterClient)
            return;
        StartCoroutine(EnemyEncounterRoutine());
    }
    IEnumerator EnemyEncounterRoutine() {
        yield return new WaitForSeconds(5);
        EndEnemyEncounter();
        yield break;
    }
    void EndEnemyEncounter() {
        photon_view.RPC("RPC_EndEnemyEncounter",RpcTarget.All);
    }
    [PunRPC] void RPC_EndEnemyEncounter() {
        GameManager.instance.is_in_event = false;
    }
}
