using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
[System.Serializable]   public class EnemyData {
    public bool in_combat;
    public Enemy[] current_enemies;
}
public class EnemyManager : MonoBehaviour
{
    [SerializeField]public EnemyData data;
    
    public Transform enemy_container;
    public PhotonView photon_view;
    
    public void UpdateState(Enemy[] new_enemies, bool is_on) {
        data.current_enemies = new_enemies;
        data.in_combat = is_on;
        if(is_on)
            StartCoroutine(HandleCombat());
    }
    #region Enemies
    public void DealDamageRandom(float damage) {
        List<Enemy> enemies_alive = new List<Enemy>();
        foreach (Enemy e in data.current_enemies)
            if(!e.current_stats.is_dead)
                enemies_alive.Add(e);
        int random_index = UnityEngine.Random.Range(0,enemies_alive.Count);
        HitEnemy(enemies_alive[random_index],damage);
    }
    public void HitEnemy(Enemy target, float damage) {
        int enemy_id = -1;
        for (int i = 0; i < data.current_enemies.Length; i++)
        {
            if(data.current_enemies[i] == target)
                enemy_id = i;
        }
        byte[] dmg_bytes = BitConverter.GetBytes(damage);
        byte[] id_bytes = BitConverter.GetBytes(enemy_id);
        photon_view.RPC("RPC_HitEnemy",RpcTarget.All,dmg_bytes,id_bytes);
    }
    [PunRPC] void RPC_HitEnemy(byte[] dmg_bytes, byte[] id_bytes) {
        float received_dmg = BitConverter.ToSingle(dmg_bytes,0);
        int received_id = BitConverter.ToInt32(id_bytes,0);
        //Get the target
        Enemy target_enemy = data.current_enemies[received_id];
        //Deal damage
        target_enemy.GetHit(received_dmg);
        Debug.Log("Hit enemy: " + target_enemy.initial_stats.name+ " for "+received_dmg);
    }
    #endregion
    IEnumerator HandleCombat() {
        GameManager.instance.TogglePlayersCombat(true);
        while(data.in_combat) {
            
            yield return null;
        }
        GameManager.instance.TogglePlayersCombat(false);
        yield break;
    }
    void OrganizeEnemies() {
        for (int i = 0; i < data.current_enemies.Length; i++)
        {
            if(i == 0)
                data.current_enemies[i].transform.position = enemy_container.position;
            else if(i%2 == 0) {
                float last_size = data.current_enemies[i-2].initial_stats.half_size;
                data.current_enemies[i].transform.position = data.current_enemies[i-2].transform.position + new Vector3(last_size+ data.current_enemies[i].initial_stats.half_size,0,0) ;// + new Vector3(i-1*i/2,0,0);
            }
            else {
                float last_size;
                if(i-2 < 0){
                    last_size = data.current_enemies[0].initial_stats.half_size;
                    data.current_enemies[i].transform.position = data.current_enemies[0].transform.position - new Vector3(last_size+ data.current_enemies[i].initial_stats.half_size,0,0) ;
                } else
                {
                    last_size = data.current_enemies[i-2].initial_stats.half_size;
                    data.current_enemies[i].transform.position = data.current_enemies[i-2].transform.position - new Vector3(last_size+ data.current_enemies[i].initial_stats.half_size,0,0) ;
                }
                
            }
        }
        Debug.Log("Organized enemy positions.");
    }
    private void Awake() {
        instance = this;
        data = new EnemyData();
        data.in_combat = false;
        data.current_enemies = new Enemy[0];
    }
    public static EnemyManager instance;
}
