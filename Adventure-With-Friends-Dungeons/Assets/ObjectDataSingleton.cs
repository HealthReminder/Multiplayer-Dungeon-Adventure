using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDataSingleton : MonoBehaviour
{
    [System.Serializable] public struct CharacterData {
        public string name;
        public Sprite sprite;
    }
    [SerializeField] public CharacterData[] available_characters;

    public static ObjectDataSingleton instance;
    private void Awake() {
        instance = this;
    }
}
