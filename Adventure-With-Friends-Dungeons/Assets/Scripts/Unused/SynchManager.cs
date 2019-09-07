using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class SynchManager : MonoBehaviour
{
    public bool is_updating;
    public int received_states = 0;
    private PhotonView view;
    public State state;
    public List<State> other_states;
    private void Awake() {
        view = GetComponent<PhotonView>();
        is_updating = false;
        state = new State();
        state.view_id = view.ViewID;
        state.times_updated = 0;
    }

    public void Schedule_Synchronization() {
        if(is_updating)
            return;
        view.RPC("RPC_Clear_States",RpcTarget.AllViaServer, BitConverter.GetBytes(UnityEngine.Random.Range(0,99999)));
        view.RPC("RPC_Send_State",RpcTarget.AllViaServer);
    }
    [PunRPC] void RPC_Clear_States(byte[] current_update) {
        other_states = new List<State>();
        received_states = 0;
        state.last_update = BitConverter.ToInt32(current_update,0);
    }
    [PunRPC] void RPC_Send_State() {
        byte[] id_bytes = BitConverter.GetBytes(state.view_id);
        byte[] up_bytes = BitConverter.GetBytes(state.last_update);
        byte[] times_byte = BitConverter.GetBytes(state.times_updated);
        view.RPC("RPC_Receive_State",RpcTarget.AllViaServer, id_bytes, up_bytes, times_byte);
    }
    [PunRPC] void RPC_Receive_State(byte[] id_bytes, byte[] up_bytes, byte[] times_byte) {
        State received_state = new State();
        received_state.view_id = BitConverter.ToInt32(id_bytes,0);
        received_state.last_update = BitConverter.ToInt32(up_bytes,0);
        received_state.times_updated = BitConverter.ToInt32(times_byte,0);
        other_states.Add(received_state);
        received_states ++;
    }

    public struct State{
        public int view_id;
        public int last_update;
        public int times_updated;
    }
}
