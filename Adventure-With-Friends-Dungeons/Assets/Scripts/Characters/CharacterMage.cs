using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class BoolEvent : UnityEvent<bool> {

}
public class CharacterMage : Character
{
    public Animator animator;
    public GameObject input_object;
    public BoolEvent on_toggle;
    public override void ToggleCombat(bool state){
        Debug.Log(state+" for mage combat GUI");
        if(state) {
            input_object.SetActive(true);
        } else
        {
            input_object.SetActive(false);
        }
        on_toggle.Invoke(state);
    }
    public override void ToggleWalk(bool state){
        animator.SetBool("is_walking",state);
        animator.SetBool("is_charging",!state);
    }
    public void OnAttack(){
        animator.SetTrigger("on_attack");
    }    
    public void OnCharge(bool state){
        animator.SetBool("is_charging",state);
        animator.SetBool("is_walking",!state);
    }
    public void OnFail(){
        animator.SetBool("is_charging",false);
        animator.SetBool("is_walking",false);
    }
    private void Update() {
        //if(Input.GetKeyDown(KeyCode.D))
        //    OnCharge(true);
        //if(Input.GetKeyDown(KeyCode.A))
        //    OnAttack();
        
    }
    
}
