using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] public class Stop
{
    public string name;
    public int distance;
    public Enemy[] enemies;
    [Header("View")]
    public GameObject stop_container;
    public ParticleSystem[] particles;
    public SpriteRenderer[] sprites;
}
[System.Serializable]  public class AdventureEvent : MonoBehaviour{
    public Stop[] stops;
    bool is_transitioning = false;
    
    public void Initiate() {
        Debug.Log("Encounter initiated"); 
        foreach (Stop stop in stops) {
            foreach (ParticleSystem p in stop.particles)
                p.Stop();
            foreach (SpriteRenderer s in stop.sprites)
                s.color += new Color(0,0,0,-1);            
        }
    }
    public IEnumerator ToggleStopRoutine(Stop stop, float step, bool is_on) {
        //Get stuff
        while(is_transitioning)
            yield return null;
        Debug.Log("Toggling adventure event to "+is_on);
        is_transitioning = true;
        SpriteRenderer[] sprites = stop.sprites;
        ParticleSystem[] particles = stop.particles;
        Enemy[] enemies = stop.enemies;
        if(is_on) {
            stop.stop_container.SetActive(false);
            //Turn enemies off
            foreach (Enemy e in enemies)
                foreach (SpriteRenderer s in e.sprt_renderers)
                    s.color += new Color(0,0,0,-1);
                    
            //Turn particles off
            foreach (ParticleSystem p in particles)
                p.Stop();
            yield return null;
             //Turn object on
            stop.stop_container.SetActive(true);

            //Show place
            if(sprites != null)
                if(sprites.Length > 0)
                    while(sprites[0].color.a < 1)   {
                        foreach (SpriteRenderer s in sprites)
                            s.color += new Color(0,0,0,0.01f*step);
                        yield return null;
                    }

            //Show Enemies
            if(enemies != null)
                if(enemies.Length > 0)
                    while(enemies[0].sprt_renderers[0].color.a < 1)   {
                        foreach (Enemy e in enemies)
                            foreach (SpriteRenderer s in e.sprt_renderers)
                                s.color += new Color(0,0,0,0.01f*step);
                        yield return null;
                    }

            //Show particles
            foreach (ParticleSystem p in particles){
                p.Play();
                yield return null;
            }
        } else {
            //Turn particles off
            foreach (ParticleSystem p in particles)
                p.Stop();
            yield return null;
            //Fade out enemies
            if(enemies != null)
                if(enemies.Length > 0)
                    while(enemies[0].sprt_renderers[0].color.a > 0)   {
                        foreach (Enemy e in enemies)
                            foreach (SpriteRenderer s in e.sprt_renderers)
                                s.color += new Color(0,0,0,-0.01f*step);
                        yield return null;
                    }
            //Fade out sprites
            if(sprites != null)
                if(sprites.Length > 0)
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