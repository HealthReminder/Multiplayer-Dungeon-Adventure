using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Serialization : MonoBehaviour
{
    [SerializeField]public PlayerData test_data;
    [SerializeField]public PlayerData received_data;
    private void Start() {
        byte[] serialized_data = SerializePlayerData(test_data);
        received_data = DeserializePlayerData(serialized_data);
    }
    
    public byte[] SerializePlayerData(PlayerData p_data) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[4][];
        arrays[0] = System.Text.Encoding.UTF8.GetBytes(p_data.player_name);
        arrays[1] = BitConverter.GetBytes(p_data.character_id);
        arrays[2] = BitConverter.GetBytes(p_data.is_playing);
        arrays[3] = BitConverter.GetBytes(p_data.is_attacking);
        Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public PlayerData DeserializePlayerData(byte[] bytes) {
        PlayerData result_data = new PlayerData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        result_data.player_name = System.Text.Encoding.UTF8.GetString(data_array[0]);
        result_data.character_id = BitConverter.ToInt32(data_array[1],0);
        result_data.is_playing = BitConverter.ToBoolean(data_array[2],0);
        result_data.is_attacking = BitConverter.ToBoolean(data_array[3],0);
        return(result_data);
    }
}
