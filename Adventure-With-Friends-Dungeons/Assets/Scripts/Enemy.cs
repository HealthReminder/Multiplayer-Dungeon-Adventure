using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;    
[Serializable]public struct InitialStats
    {
        public string name;
        public float initial_hp;
        public int level;
        public float half_size;
    }
    [Serializable]public struct CurrentStats
    {
        public bool is_awakened;
        public bool is_dead;
        public float current_hp;
    }
public class Enemy : MonoBehaviour
{
    public bool is_dead = false;
    //public PlayerManager current_target;
    public SpriteRenderer[] sprt_renderers;
    public EnemyView view;
    [Header("State")]
    [SerializeField]public InitialStats initial_stats;
    [SerializeField]public CurrentStats current_stats;



#region Player Reaction
    public void OnAwakened() {
        view.OnAwaken();
    }
    public void GetHit(float dmg) {
        if(is_dead)
            return;

        current_stats.current_hp-=dmg;
        if(current_stats.current_hp<=0)
            Die();
        else
            view.OnHit();
    }
    public void Die() {
        is_dead = true;
        view.OnDie();
    }
#endregion
#region Start
    public void Setup() {
        current_stats.is_awakened = false;
        current_stats.is_dead = false;
        current_stats.current_hp = initial_stats.initial_hp;
    }
#endregion
    private void Awake() {
        Setup();
    }
}
