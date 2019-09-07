using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectIndex : MonoBehaviour
{
    [System.Serializable] public struct CharacterData {
        public string name;
        public Sprite sprite;
    }
    [SerializeField] public CharacterData[] available_characters;

    public static ObjectIndex instance;
    private void Awake() {
        instance = this;
    }
}
