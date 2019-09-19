using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;    
public class Enemy : MonoBehaviour
{
    [Serializable]public struct InitialStats
    {
        public string name;
        public float initial_hp;
        public int level;
        public float half_size;
    }
    [Serializable]public struct CurrentStats
    {
        public bool is_dead;
        public float current_hp;

    }
    [SerializeField]public InitialStats initial_stats;
    [SerializeField]public CurrentStats current_stats;

    public bool is_dead = false;
    //public PlayerManager current_target;
    public SpriteRenderer sprt_renderer;

#region Player Reaction
    public void GetHit(float dmg) {
        if(is_dead)
            return;

        current_stats.current_hp-=dmg;
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
    private void Awake() {
        Setup();
    }
}
