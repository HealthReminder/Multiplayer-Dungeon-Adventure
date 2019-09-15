﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundView : MonoBehaviour
{
    struct BackgroundObject {
        public GameObject gameObject;
        public float percentage;
        public SpriteRenderer sprt;
    }
    [Range(0,2)] public float current_speed = 3;
    public Sprite[] available_sprites;
    BackgroundObject[] scenary_objects;
    public AnimationCurve movement_curve;
    public AnimationCurve size_curve;
    private void Start() {
        Setup(36);
        StartCoroutine(MovementRoutine());
    }
    public void ToggleMovement(float target_speed) {
        StartCoroutine(ToggleMovementRoutine(target_speed,1));
    }
    [Range(-0.5f,0.5f)] 
    public float min_y;
    public float offsetx,max_x;
    bool is_moving = false;
    public IEnumerator ToggleMovementRoutine(float target_speed,float step) {
        is_moving = false;
        yield return null;
        is_moving = true;
        
        if(current_speed < target_speed){
            while(current_speed < target_speed && is_moving) {
                current_speed+=step*Time.deltaTime;
                yield return null;
            }
        } else if (current_speed > target_speed && is_moving) {
            while(current_speed > target_speed) {
                current_speed-=step*Time.deltaTime;
                yield return null;
            }
        }
        current_speed = target_speed;
        yield break;
    }
    float time = 0;
    IEnumerator MovementRoutine() {
        while(true){
            for (int i = 0; i < scenary_objects.Length; i++)
            {
                scenary_objects[i].percentage += Time.deltaTime*current_speed;
                if(scenary_objects[i].percentage > 1){
                    scenary_objects[i].percentage = 0;
                    int new_sprite_index = Random.Range(0,available_sprites.Length);
                    scenary_objects[i].sprt.sprite = available_sprites[new_sprite_index];
                }
                Transform t = scenary_objects[i].gameObject.transform;
                if(i%2 == 0)
                    t.position = transform.position + new Vector3(-offsetx-scenary_objects[i].percentage*max_x,movement_curve.Evaluate(1-scenary_objects[i].percentage)*min_y,0);
                else
                    t.position = transform.position + new Vector3(offsetx+scenary_objects[i].percentage*max_x,movement_curve.Evaluate(1-scenary_objects[i].percentage)*min_y,0);
                
                t.localScale = new Vector3(3*size_curve.Evaluate(scenary_objects[i].percentage),3*size_curve.Evaluate(scenary_objects[i].percentage),1);
                scenary_objects[i].sprt.sortingOrder = (int)((scenary_objects[i].percentage*100)-100);
                //scenary_objects[i].sprt.color = new Color(1*size_curve.Evaluate(scenary_objects[i].percentage),1*size_curve.Evaluate(scenary_objects[i].percentage),1*size_curve.Evaluate(scenary_objects[i].percentage),1);
                time+=Time.deltaTime;
            }
            yield return null;
        }
    }
    private void Setup(int objectQuantity) {
        Debug.Log("Setup BackgroundView");
        float current_percentage = 0;
        float offset_objects = 1f/objectQuantity;
        //min_y = -3;
        //offsetx = 1;
        //max_x = 10;

        scenary_objects = new BackgroundObject[objectQuantity];
        for (int i = 0; i < scenary_objects.Length; i++)
        {
            //print(current_percentage +" "+ movement_curve.Evaluate(current_percentage) +" "+ offset_objects);
            BackgroundObject o = new BackgroundObject();
            o.percentage = current_percentage;
            o.gameObject = new GameObject(i+"ScenaryObject");
            o.gameObject.transform.parent = transform;
            o.sprt = o.gameObject.AddComponent<SpriteRenderer>();
            o.sprt.sprite = available_sprites[Random.Range(0,available_sprites.Length)];
            scenary_objects[i] = o;
            Transform t = o.gameObject.transform;
            if(i%2 == 0)
                t.position = transform.position + new Vector3(-offsetx-current_percentage*max_x,movement_curve.Evaluate(1-current_percentage)*min_y,0);
            else
                t.position = transform.position + new Vector3(offsetx+current_percentage*max_x,movement_curve.Evaluate(1-current_percentage)*min_y,0);
            
            t.localScale = new Vector3(3*size_curve.Evaluate(current_percentage),3*size_curve.Evaluate(current_percentage),1);
            o.gameObject.SetActive(true);
            o.percentage = current_percentage;
            
            if(i%2 != 0)
                current_percentage += offset_objects*2;
        }
    }
    private void Awake() {
        instance = this;
    }
    public static BackgroundView instance;
}