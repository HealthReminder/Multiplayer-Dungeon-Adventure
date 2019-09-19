using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Serialization : MonoBehaviour
{    
    //[SerializeField] public EnemyData serialize_event;
    //[SerializeField] public EnemyData deserialized_event;
    private void Update() {
        //if(Input.GetKeyDown(KeyCode.A))
        //    deserialized_event = DeserializeEnemyData(SerializeEnemyData(serialize_event,EnemyManager.instance.available_enemies),EnemyManager.instance.available_enemies);
    }
    #region GameManager
    public byte[] SerializeGameData(GameData g_data) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[2][];
        arrays[0] = BitConverter.GetBytes(g_data.is_adventure_started);
        arrays[1] = BitConverter.GetBytes(g_data.players_in_room);
        Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public GameData DeserializeGameData(byte[] bytes) {
        GameData result_data = new GameData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        result_data.is_adventure_started = BitConverter.ToBoolean(data_array[0],0);
        result_data.players_in_room = BitConverter.ToInt32(data_array[1],0);
        return(result_data);
    }
    #endregion
    #region EnemyManager
    public byte[] SerializeEnemyData(EnemyData e_data, Enemy[] enemies_available) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[e_data.current_enemies.Length+2][];
        arrays[0] = BitConverter.GetBytes(e_data.in_combat);
        arrays[1] = BitConverter.GetBytes(e_data.current_enemies.Length);
        for (int i = 0; i < e_data.current_enemies.Length; i++)
            if(e_data.current_enemies[i] != null){
                for (int o = 0; o < enemies_available.Length; o++)
                    if(enemies_available[o] == e_data.current_enemies[i])
                        arrays[i+2] = BitConverter.GetBytes(o);   
            } else 
                arrays[i+2] = BitConverter.GetBytes((int)(-1));

        Debug.Log("There was "+e_data.current_enemies.Length+" enemies.");
        Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public EnemyData DeserializeEnemyData(byte[] bytes, Enemy[] enemies_available) {
        EnemyData result_data = new EnemyData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        result_data.in_combat = BitConverter.ToBoolean(data_array[0],0);
        int enemy_quantity = BitConverter.ToInt32(data_array[1],0);
        Debug.Log("There are "+enemy_quantity+" enemies");
        Enemy[] new_enemy_list = new Enemy[enemy_quantity];
        for (int i = 2; i < enemy_quantity+2; i++){
            int character_id = BitConverter.ToInt32(data_array[i],0);
            //if(character_id != -1)
                //new_enemy_list.Add(enemies_available[character_id]);
        }
        result_data.current_enemies = new_enemy_list;
        return(result_data);
    }
    #endregion
    #region EventManager
    public byte[] SerializeEventData(EventData e_data) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[2][];
        arrays[0] = BitConverter.GetBytes(e_data.current_event_id);
        arrays[1] = BitConverter.GetBytes(e_data.is_in_event);
        Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public EventData DeserializeEventData(byte[] bytes) {
        EventData result_data = new EventData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        result_data.current_event_id = BitConverter.ToInt32(data_array[0],0);
        result_data.is_in_event = BitConverter.ToBoolean(data_array[1],0);
        return(result_data);
    }
    #endregion
    #region PlayerData
    public byte[] SerializePlayerData(PlayerData p_data) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[5][];
        arrays[0] = System.Text.Encoding.UTF8.GetBytes(p_data.player_name);
        arrays[1] = BitConverter.GetBytes(p_data.photon_view_id);
        arrays[2] = BitConverter.GetBytes(p_data.player_id);
        arrays[3] = BitConverter.GetBytes(p_data.character_id);
        arrays[4] = BitConverter.GetBytes(p_data.is_playing);
        Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public PlayerData DeserializePlayerData(byte[] bytes) {
        PlayerData result_data = new PlayerData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        result_data.player_name = System.Text.Encoding.UTF8.GetString(data_array[0]);
        result_data.photon_view_id = BitConverter.ToInt32(data_array[1],0);
        result_data.player_id = BitConverter.ToInt32(data_array[2],0);
        result_data.character_id = BitConverter.ToInt32(data_array[3],0);
        result_data.is_playing = BitConverter.ToBoolean(data_array[4],0);
        return(result_data);
    }
    #endregion
    
    public static Serialization instance;
    private void Awake() {
        instance = this;
    }
}
