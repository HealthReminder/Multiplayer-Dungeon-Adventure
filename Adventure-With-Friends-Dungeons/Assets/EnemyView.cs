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
    public SpriteRenderer[] sprites;
    private void Update() {
        //if(Input.GetKeyDown(KeyCode.U))
        //    OnAwaken();
        //if(Input.GetKeyDown(KeyCode.I))
        //    OnHit();
        //if(Input.GetKeyDown(KeyCode.O))
        //    OnDie();
    }
    public void OnAwaken() {
        animator.SetTrigger("is_aroused");
        if(on_awakened != null)
            on_awakened.Invoke();
    }
    public void OnHit() {
        animator.SetTrigger("is_hit");
        if(on_hit != null)
            on_hit.Invoke();
    }
    public void OnDie(){
        animator.SetTrigger("is_dead");
        if(on_dead != null)
            on_dead.Invoke();
        StartCoroutine(FadeOut(1));
    }
    IEnumerator FadeOut(float delay) {
        yield return new WaitForSeconds(delay);
        if(sprites!=null)
            if(sprites.Length > 0)
            while(sprites[0].color.a > 0) {
                foreach (SpriteRenderer s in sprites)
                    s.color += new Color(0,0,0,-0.01f);
                yield return null;
            }
        yield break;
    }
    
}
