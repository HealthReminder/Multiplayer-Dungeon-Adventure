using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMage : Character
{
    public override void ToggleCombat(bool is_on){
        Debug.Log(is_on+" for mage combat GUI");
    }
}
