using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleBehaviour : MonoBehaviour
{
    public float wobble_velocity = 1;
    private void Start() {
        transform.localPosition = new Vector3(0,Mathf.PerlinNoise(Random.Range(0f,1f),0)*wobble_velocity*0.25f,0);
    }
    private void Update() {
        transform.localPosition = new Vector3(0,Mathf.PerlinNoise(Time.time,0)*wobble_velocity*0.25f,0);
    }
}
