using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] public struct Place
{
    public string name;
    public GameObject place_container;
    public ParticleSystem[] p_s;
    public SpriteRenderer[] sprites;
    public int distance;
}
[System.Serializable]  public class AdventureEvent : MonoBehaviour{
    public GameObject[] enemies;
    public Place[] places;
    bool is_transitioning = false;
    
    public void Initiate() {
        Debug.Log("Encounter initiated"); 
        foreach (Place pl in places) {
            foreach (ParticleSystem p in pl.p_s)
                p.Stop();
            foreach (SpriteRenderer s in pl.sprites)
                s.color += new Color(0,0,0,-1);            
        }
            
    }
    public IEnumerator TogglePlaceRoutine(Place place, float step, bool is_on) {
        //Get stuff
        while(is_transitioning)
            yield return null;
        Debug.Log("Toggling adventure event to "+is_on);
        is_transitioning = true;
        SpriteRenderer[] sprites = place.sprites;
        ParticleSystem[] particles = place.p_s;
        if(is_on) {
            place.place_container.SetActive(false);
            //Turn particles off
            foreach (ParticleSystem p in particles)
                p.Stop();
            yield return null;
            place.place_container.SetActive(true);
            //Fade in sprites
            while(sprites[0].color.a < 1)   {
                foreach (SpriteRenderer s in sprites)
                    s.color += new Color(0,0,0,0.01f*step);
                yield return null;
            }
            yield return null;
            //Start particles
            foreach (ParticleSystem p in particles){
                p.Play();
                yield return null;
            }
        } else {
            //Turn particles off
            foreach (ParticleSystem p in particles)
                p.Stop();
            yield return null;
            //Fade out sprites
            while(sprites[0].color.a > 0)   {
                foreach (SpriteRenderer s in sprites)
                    s.color += new Color(0,0,0,-0.01f*step);
                yield return null;
            }
        }
        is_transitioning = false;
        yield break;
    }
}