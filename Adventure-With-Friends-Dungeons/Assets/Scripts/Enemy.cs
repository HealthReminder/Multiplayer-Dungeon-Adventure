using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;    
public class Enemy : MonoBehaviour
{
    [Serializable]public struct InitialStats
    {
        public float initial_hp;
        public int level;
    }
    struct CurrentStats
    {
        public bool is_dead;
        public float current_hp;

    }
    [SerializeField]public InitialStats initial_stats;
    CurrentStats current_stats;

    public PhotonView photon_view;
    public bool is_dead = false;
    public PlayerManager current_target;

#region Player Reaction
    public void GetHit(float dmg) {
        if(is_dead)
            return;

        byte[] dmg_bytes = BitConverter.GetBytes(dmg);
        photon_view.RPC("RPC_GetHit",RpcTarget.All,dmg_bytes);
    }
    [PunRPC] void RPC_GetHit(byte[] dmg_bytes) {
        if(is_dead)
            return;

        float r_dmg = BitConverter.ToSingle(dmg_bytes,0);

        current_stats.current_hp-=r_dmg;
        if(current_stats.current_hp<=0)
            Die();
    }
    public void Die() {
        is_dead = true;
    }
#endregion
#region Start
    public void Setup() {
        current_stats.is_dead = false;
        current_stats.current_hp = initial_stats.initial_hp;
    }
#endregion
}
