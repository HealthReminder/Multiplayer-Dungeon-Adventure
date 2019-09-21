using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Character resets the behaviour on start
//Player can roll dice to increase value
//Player can end turn to stop input and attack with current value
//If player rolls too high its turn fails, end
//If player rolls to cap it attacks with double damage, end
public class MagicBarView : MonoBehaviour
{
    public bool is_on = false;
    public bool is_enabled = false;
    public int cap = 13;
    public int current_value = 0;
    public CharacterMage character;
    public Image filler_image;
    public Button roll_button,attack_button;
    
    private void Awake() {
        Reset(false);
    }
    public void RollDice() {
        if(!is_enabled)
            return;
        int roll = Random.Range(1,7);

        current_value+= roll;
        GUI_Update();
        if(current_value > cap)
            OnOverflow();
        else if(current_value == cap)
            OnJackpot();
        else 
            GUI_Update();
        OnRollDice();
    }
#region GUI
    void GUI_Update() {
        filler_image.fillAmount = (float)current_value/(float)cap;
    }
    void GUI_JackPot() {
        filler_image.fillAmount = 1;
        GUI_ToggleInput(false);
    }
    void GUI_Overflow() {
        filler_image.fillAmount = 0;
        GUI_ToggleInput(false);
    }
    void GUI_Attack() {
        filler_image.fillAmount = 0;
        GUI_ToggleInput(false);
    }
    void GUI_ToggleInput(bool new_state){
        roll_button.interactable = new_state;
        attack_button.interactable = new_state;

    }
#endregion
#region INPUT
    public void OnAttack() {
        if(!is_enabled)
            return;
        GUI_Attack();
        character.OnCharge(false);
        character.OnAttack();
        is_enabled = false;
        EnemyManager.instance.DealDamageRandom(current_value);
        StartCoroutine(TurnOnCooldown(1.5f));
        Debug.Log("Mage released its power!");
    }
#endregion
#region CONSEQUENCES
    void OnOverflow() {
        is_enabled = false;
        GUI_Overflow();
        character.OnFail();
        StartCoroutine(TurnOnCooldown(3));
        Debug.Log("Mage overflowed its power.");
    }
    void OnRollDice() {
        Debug.Log("Mage rolled a dice.");
        character.OnCharge(true);
    }
    IEnumerator TurnOnCooldown(float time){
        yield return new WaitForSeconds(time);
        if(is_on)
            Reset(true);
    }
    void OnJackpot() {
        is_enabled = false;
        character.OnCharge(false);
        character.OnAttack();
        GUI_JackPot();
        is_enabled = false;
        EnemyManager.instance.DealDamageRandom(current_value*2);
        StartCoroutine(TurnOnCooldown(1f));
        Debug.Log("Mage released its full power!");
    }
#endregion
#region SETUP
    public void Reset(bool state) {
        current_value = 0;
        GUI_Update();
        GUI_ToggleInput(state);
        is_enabled = state;
        is_on = state;
        character.OnFail();
    }
#endregion
}
