using UnityEngine;
using UnityEngine.UI;

//Character resets the behaviour on start
//Player can roll dice to increase value
//Player can end turn to stop input and attack with current value
//If player rolls too high its turn fails, end
//If player rolls to cap it attacks with double damage, end
public class MagicBarView : MonoBehaviour
{
    public bool is_on = false;
    public int cap = 13;
    public int current_value = 0;
    public Image filler_image;
    public Button roll_button,attack_button;
    private void Awake() {
        Reset(false);
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.A))
            Reset(true);
    }
    public void RollDice() {
        if(!is_on)
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
    }
#region GUI
    public void GUI_Update() {
        filler_image.fillAmount = (float)current_value/(float)cap;
    }
    public void GUI_JackPot() {
        filler_image.fillAmount = 1;
        GUI_ToggleInput(false);
    }
    public void GUI_Overflow() {
        filler_image.fillAmount = 0;
        GUI_ToggleInput(false);
    }
    public void GUI_Attack() {
        filler_image.fillAmount = 0;
        GUI_ToggleInput(false);
    }
    public void GUI_ToggleInput(bool new_state){
        roll_button.interactable = new_state;
        attack_button.interactable = new_state;

    }
#endregion
#region INPUT
    public void OnAttack() {
        if(!is_on)
            return;
        GUI_Attack();
        is_on = false;
        Debug.Log("Mage released its power!");
    }
#endregion
#region CONSEQUENCES
    public void OnOverflow() {
        is_on = false;
        GUI_Overflow();
        Debug.Log("Mage overflowed its power.");
    }
    public void OnJackpot() {
        is_on = false;
        GUI_JackPot();
        Debug.Log("Mage released its full power!");
    }
#endregion
#region SETUP
    public void Reset(bool state) {
        current_value = 0;
        GUI_Update();
        GUI_ToggleInput(state);
        is_on = state;
        Debug.Log("Mage overflowed its power.");
    }
#endregion
}
