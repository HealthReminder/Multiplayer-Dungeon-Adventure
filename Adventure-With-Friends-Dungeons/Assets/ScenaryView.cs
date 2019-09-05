using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenaryView : MonoBehaviour
{
    public Sprite sprite_test;
    public int qtd_at_once_test = 3;
    public GameObject object_template;
    public AnimationCurve vertical_movement_curve;
    public bool is_moving = false;
    List<Transform> current_objects;
    private void Awake() {
        SetupScenary(sprite_test,qtd_at_once_test);
    }
    void SetupScenary(Sprite object_sprite, float qtd_at_once){
        if(current_objects == null)
            current_objects = new List<Transform>();
        //Remove extra objects
        while(current_objects.Count > qtd_at_once){
            Destroy(current_objects[current_objects.Count-1].gameObject);
            current_objects.RemoveAt(current_objects.Count-1);
        }
        //Add missing objects
        while(current_objects.Count < qtd_at_once){
            GameObject new_object = Instantiate(object_template,transform.position, Quaternion.identity);
            Transform new_transform = new_object.transform;
            new_transform.parent = transform;
            current_objects.Add(new_transform);
        }
    }
    int current_index;
    private void Update() {
        if(is_moving)
            MoveScenary(qtd_at_once_test);
    }
    void MoveScenary(int qtd) {
        float max_x = 3;
        for (int i = 0; i < current_objects.Count; i++)
        {
            //current_objects[i].transform.position = new Vector3(max_x*)

        }
    }
}
