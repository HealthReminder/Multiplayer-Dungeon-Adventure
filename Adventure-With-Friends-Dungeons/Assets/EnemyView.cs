using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StringEvent : UnityEvent<string>
{
}
public class EnemyView : MonoBehaviour
{
    //The enemy view will have several events that can call animations or custom behaviours
    public Animator animator;
    [SerializeField]UnityEvent on_awakened;
    [SerializeField]UnityEvent on_hit;
    [SerializeField]UnityEvent on_dead;
    private void Update() {
        if(Input.GetKeyDown(KeyCode.U))
            OnDie();
        if(Input.GetKeyDown(KeyCode.I))
            OnAwaken();
        if(Input.GetKeyDown(KeyCode.O))
            OnDie();
    }
    public void OnAwaken() {
        animator.SetTrigger("is_aroused");
        on_awakened.Invoke();
    }
    public void OnHit() {
        animator.SetTrigger("is_hit");
        on_hit.Invoke();
    }
    public void OnDie(){
        animator.SetTrigger("is_dead");
        on_dead.Invoke();
    }
    
}
