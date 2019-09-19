using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]   public class EnemyData {
    public bool in_combat;
    public Enemy[] current_enemies;
}
public class EnemyManager : MonoBehaviour
{
    [SerializeField]public EnemyData data;
    
    public Transform enemy_container;
    
    public void UpdateState(Enemy[] new_enemies, bool is_on) {
        data.current_enemies = new_enemies;
        data.in_combat = is_on;
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
