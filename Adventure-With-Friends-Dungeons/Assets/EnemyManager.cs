using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]   public class EnemyData {
    public bool in_combat;
    public List<Enemy> current_enemies;
}
public class EnemyManager : MonoBehaviour
{
    [SerializeField]public EnemyData data;
    public Enemy[] available_enemies;
    
    public Transform enemy_container;
    
    private void Update() {
        if(Input.GetKeyDown(KeyCode.G))
            CalculateEnemies(6);
        if(Input.GetKeyDown(KeyCode.C))
            Reset();
    }
    public IEnumerator ToggleCombat(bool is_on) {
        data.in_combat = is_on;
        if(is_on){
            if(data.current_enemies[0] != null)
            while(data.current_enemies[0].sprt_renderer.color.a < 1){
                foreach (Enemy e in data.current_enemies)
                    e.sprt_renderer.color += new Color(0,0,0,0.05f);
                yield return null;
            }         
        } else {
            if(data.current_enemies[0] != null)
            while(data.current_enemies[0].sprt_renderer.color.a > 0){
                foreach (Enemy e in data.current_enemies)
                    e.sprt_renderer.color += new Color(0,0,0,-0.05f);
                yield return null;
            }
        }
        yield break;
    }
    public Enemy[] CalculateEnemies(int target_threat_level) {
        Reset();
        List<Enemy> available_list = new List<Enemy>();
        List<Enemy> final_list = new List<Enemy>();
        //Reference all enemies available
        for (int i = 0; i < available_enemies.Length; i++)
            available_list.Add(available_enemies[i]);
        //Take out the ones higher than the threat level
        for (int i = available_list.Count-1; i >=0 ; i--)
            if(available_list[i].initial_stats.level > target_threat_level)
                available_list.RemoveAt(i);
        //Get a random asort of enemies and store them
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
        return (final_list.ToArray());
    }   
    public void GenerateCombat(Enemy[] new_enemies) {
        //This function is called in every computer after the master had already calculated the enemies
        Debug.Log("Generated "+new_enemies.Length+" enemies.");
        data.current_enemies = new List<Enemy>();
        for (int i = 0; i < new_enemies.Length; i++)
        {
            GameObject new_enemy = Instantiate(new_enemies[i].gameObject,transform.position,Quaternion.identity);
            new_enemy.transform.localScale = new Vector3(Random.Range(0,2)*2-1,1,1);
            new_enemy.transform.parent = enemy_container;
            data.current_enemies.Add(new_enemy.GetComponent<Enemy>());
        }
        OrganizeEnemies();
    }
    void OrganizeEnemies() {
        for (int i = 0; i < data.current_enemies.Count; i++)
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
    void Reset() {
        if(data.current_enemies != null)
            for (int i = data.current_enemies.Count-1; i >= 0; i--)
                if(data.current_enemies[i])
                    Destroy(data.current_enemies[i].gameObject);
        data.current_enemies = new List<Enemy>();
        Debug.Log("Reseted enemy manager.");
    }
    private void Awake() {
        instance = this;
        data = new EnemyData();
        data.in_combat = false;
        data.current_enemies = new List<Enemy>();
    }
    public static EnemyManager instance;
}
