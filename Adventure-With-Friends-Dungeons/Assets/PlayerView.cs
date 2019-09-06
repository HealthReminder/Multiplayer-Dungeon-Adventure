using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    public PlayerManager player_manager;
    [Header("Start Adventure")]
    public GameObject start_input_container;
    [Header("Char Selection")]
    public GameObject[] chars_available;

    [Header("Overlay")]
    public Image overlay_img;
    public Text waiting_text;
    

    //START
    public void ToggleStartInput(bool is_on) {
        Debug.Log("Toggled start input for "+is_on);
        if(is_on)
            start_input_container.SetActive(true);
        else
            start_input_container.SetActive(false);
    }
    
    public void FadeOverlay(float fade_to, float step) {
        StartCoroutine(FadeOverlayRoutine(fade_to,step));
    }
    IEnumerator FadeOverlayRoutine(float fade_to, float step) {
        if(overlay_img.color.a < fade_to)
            while(overlay_img.color.a < fade_to){
                overlay_img.color += new Color(0,0,0,step*Time.deltaTime);
                waiting_text.color += new Color(0,0,0,step*Time.deltaTime);
                yield return null;
            }
        else 
            while(overlay_img.color.a > fade_to){
                overlay_img.color += new Color(0,0,0,-step*Time.deltaTime);
                waiting_text.color += new Color(0,0,0,-step*Time.deltaTime);
                yield return null;
            }
        overlay_img.color += new Color(0,0,0,fade_to);
        yield break;
    }

    //CHARACTER SELECTION
    [Header("Character Selection")]
    public SpriteRenderer character_avatar_rndr;
    
    public void ToggleAvatar(int char_id) {
        chars_available[char_id].SetActive(true);
    }
    public GameObject character_selection_container;
    int times_can_choose_character = 1;
    int times_chosen_character = 0;
    public void ToggleCharacterSelection(int is_on) {
        if(is_on == 0)
            character_selection_container.SetActive(false);
        else
            character_selection_container.SetActive(true);
    }
    public void Choose_Character(int character_id) {
        if(!player_manager)
            return;
        if(times_chosen_character >= times_can_choose_character)
            return;
        ToggleCharacterSelection(0);
        times_chosen_character ++;
        player_manager.ChooseCharacter(character_id);
    }
}
