using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] public struct Place
{
    public Sprite sprite;
    public int distance;
}
[System.Serializable]  public class AdventureEvent {
    public GameObject[] enemies;
    public Place[] places;
    public void Initiate() {
        Debug.Log("Encounter initiated");
        //PlayerManager[] ps = GameObject.FindObjectsOfType<PlayerManager>();
        //foreach (PlayerManager p in ps)
    
    }
}