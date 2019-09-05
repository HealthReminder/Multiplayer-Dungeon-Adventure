﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenaryTest : MonoBehaviour
{
    public GameObject obj_prefab;
    struct ScenaryObject {
        public GameObject gameObject;
        public float percentage;
        public SpriteRenderer sprt;
    }
    public bool is_moving = true;
    public float speed = 3;
    ScenaryObject[] scenary_objects;
    public AnimationCurve movement_curve;
    public AnimationCurve size_curve;
    private void Start() {
        Setup(10);
        StartCoroutine(MovementRoutine());
    }
    [Range(-0.5f,0.5f)] 
    public float min_y;
    public float offsetx,max_x;
    IEnumerator MovementRoutine() {
        while(true){
            if(is_moving){
                for (int i = 0; i < scenary_objects.Length; i++)
                {
                    scenary_objects[i].percentage += Time.deltaTime*speed;
                    if(scenary_objects[i].percentage > 1)
                        scenary_objects[i].percentage = 0;
                    Transform t = scenary_objects[i].gameObject.transform;
                    if(i%2 == 0)
                        t.position = transform.position + new Vector3(-offsetx-scenary_objects[i].percentage*max_x,movement_curve.Evaluate(1-scenary_objects[i].percentage)*min_y,0);
                    else
                        t.position = transform.position + new Vector3(offsetx+scenary_objects[i].percentage*max_x,movement_curve.Evaluate(1-scenary_objects[i].percentage)*min_y,0);
                    
                    t.localScale = new Vector3(3*size_curve.Evaluate(scenary_objects[i].percentage),3*size_curve.Evaluate(scenary_objects[i].percentage),1);
                    scenary_objects[i].sprt.sortingOrder = (int)((scenary_objects[i].percentage*100)-100);
                    scenary_objects[i].sprt.color = new Color(1*size_curve.Evaluate(scenary_objects[i].percentage),1*size_curve.Evaluate(scenary_objects[i].percentage),1*size_curve.Evaluate(scenary_objects[i].percentage),1);
                    
                }
                yield return null;
            } else{
                yield return null;
            }
            yield return null;
        }
    }
    private void Setup(int objectQuantity) {

        float current_percentage = 0;
        float offset_objects = 1f/objectQuantity;
        //min_y = -3;
        //offsetx = 1;
        //max_x = 10;

        scenary_objects = new ScenaryObject[objectQuantity];
        for (int i = 0; i < scenary_objects.Length; i++)
        {
            print(current_percentage +" "+ movement_curve.Evaluate(current_percentage) +" "+ offset_objects);
            ScenaryObject o = new ScenaryObject();
            o.percentage = current_percentage;
            o.gameObject = Instantiate(obj_prefab,transform.position,Quaternion.identity);
            o.sprt = o.gameObject.GetComponent<SpriteRenderer>();
            scenary_objects[i] = o;
            Transform t = scenary_objects[i].gameObject.transform;
            if(i%2 == 0)
                t.position = transform.position + new Vector3(-offsetx-current_percentage*max_x,movement_curve.Evaluate(1-current_percentage)*min_y,0);
            else
                t.position = transform.position + new Vector3(offsetx+current_percentage*max_x,movement_curve.Evaluate(1-current_percentage)*min_y,0);
            
            t.localScale = new Vector3(3*size_curve.Evaluate(current_percentage),3*size_curve.Evaluate(current_percentage),1);
            scenary_objects[i].gameObject.SetActive(true);
            scenary_objects[i].percentage = current_percentage;
            
            if(i%2 != 0)
                current_percentage += offset_objects*2;
        }
    }
}