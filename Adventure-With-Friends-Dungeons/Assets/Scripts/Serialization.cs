using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Serialization : MonoBehaviour
{    
    [SerializeField] public EventData serialize_event;
    [SerializeField] public EventData deserialized_event;
    private void Start() {
        deserialized_event = DeserializeEventData(SerializeEventData(serialize_event));
    }
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
        byte[][] arrays = new byte[6][];
        arrays[0] = System.Text.Encoding.UTF8.GetBytes(p_data.player_name);
        arrays[1] = BitConverter.GetBytes(p_data.photon_view_id);
        arrays[2] = BitConverter.GetBytes(p_data.player_id);
        arrays[3] = BitConverter.GetBytes(p_data.character_id);
        arrays[4] = BitConverter.GetBytes(p_data.is_playing);
        arrays[5] = BitConverter.GetBytes(p_data.is_attacking);
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
        result_data.is_attacking = BitConverter.ToBoolean(data_array[5],0);
        return(result_data);
    }
    #endregion
    
    public static Serialization instance;
    private void Awake() {
        instance = this;
    }
}
