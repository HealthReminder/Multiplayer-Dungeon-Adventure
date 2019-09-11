using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]  public class Encounter {
    public GameObject[] enemies;
    public GameObject[] enemies_all;
    public List<GameObject> enemies_alive;
    public void Initiate() {
        Debug.Log("Encounter initiated");
        //PlayerManager[] ps = GameObject.FindObjectsOfType<PlayerManager>();
        //foreach (PlayerManager p in ps)
    
    }
}