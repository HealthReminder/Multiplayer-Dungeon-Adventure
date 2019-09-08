using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField] GameObject chat_container;
    [SerializeField] GameObject chat_entry;
    [SerializeField] List<GameObject> current_entries;
#region During Adventure
    public void AddEntry(string player_name,string message){
        
    } 
#endregion

#region On Start
    private void Awake() {
        Setup();
    }
    private void Setup(){
        current_entries = new List<GameObject>();
    }
#endregion

}
