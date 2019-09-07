using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCleric : Character
{
    public override void ToggleCombat(bool is_on){
        Debug.Log(is_on+" for cleric combat GUI");
    }
}

