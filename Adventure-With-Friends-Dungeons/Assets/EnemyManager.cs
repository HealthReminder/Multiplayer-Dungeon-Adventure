using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Enemy[] available_enemies;
    public List<Enemy> current_enemies;
    
    private void Update() {
        if(Input.GetKeyDown(KeyCode.G))
            GenerateCombat(3);
        if(Input.GetKeyDown(KeyCode.C))
            Reset();
    }
    public void GenerateCombat(int target_threat_level) {
        Reset();
        List<Enemy> available_list = new List<Enemy>();
        List<Enemy> final_list = new List<Enemy>();
        for (int i = 0; i < available_enemies.Length; i++)
            available_list.Add(available_enemies[i]);
        //Get enemies that are less or equal threat_level
        for (int i = available_list.Count-1; i >=0 ; i--)
            if(available_list[i].initial_stats.level > target_threat_level)
                available_list.RemoveAt(i);
        //Get random enemies
        int current_threat_level = 0;
        int tries = 0;
        while(current_threat_level <= target_threat_level && tries < 50) {
            int r = Random.Range(0,available_list.Count);
            if(current_threat_level + available_list[r].initial_stats.level <= target_threat_level) {
                current_threat_level += available_list[r].initial_stats.level;
                final_list.Add(available_list[r]);
            }
            tries++;
        }
        Debug.Log("Generated "+final_list.Count+" enemies.");
        current_enemies = new List<Enemy>();
        for (int i = 0; i < final_list.Count; i++)
        {
            current_enemies.Add(Instantiate(final_list[i].gameObject,transform.position,Quaternion.identity).GetComponent<Enemy>());
            current_enemies[i].transform.parent = transform;
        }
        
    }   
    void Reset() {
        if(current_enemies != null)
            for (int i = current_enemies.Count-1; i >= 0; i--)
                if(current_enemies[i])
                    Destroy(current_enemies[i].gameObject);
        current_enemies = new List<Enemy>();
    }
}
