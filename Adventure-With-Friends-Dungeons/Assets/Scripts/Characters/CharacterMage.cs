using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class BoolEvent : UnityEvent<bool> {

}
public class CharacterMage : Character
{
    public GameObject input_object;
    public BoolEvent on_toggle;
    public override void ToggleCombat(bool is_on){
        Debug.Log(is_on+" for mage combat GUI");
        if(is_on) {
            input_object.SetActive(true);
        } else
        {
            input_object.SetActive(false);
        }
        on_toggle.Invoke(is_on);
    }
}
