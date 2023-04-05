using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class ControllerScript : MonoBehaviour
{
    public void PlayAll()
    {
        var cs_array = GameObject.FindObjectsOfType<CharScript>();
        foreach(var cs in cs_array)
        {
            PlayCharacter(cs, cs.animID);
        }
    }
    public void PlayCharacter(CharScript cs, string animID)
    {
        if (cs.playableGraph.IsValid())
        {
            for (int i = 0; i < cs.playableGraph.GetRootPlayableCount(); i++)
            {
                var playable = cs.playableGraph.GetRootPlayable(i);
                playable.SetTime(0.0d);
                if (playable.GetPlayState() == PlayState.Paused)
                {
                    playable.Play();
                }

            }
            cs.playableGraph.Play();

        }
        else
        {
            cs.LoadAnim(animID);
            cs.playableGraph.Play();
        }
    }
    public void StopAll()
    {
        var cs_array = GameObject.FindObjectsOfType<CharScript>();
        foreach (var cs in cs_array)
        {
            if (cs.playableGraph.IsValid())
            {
                cs.playableGraph.Stop();
            }
        }
    }
    public void PauseCharacter(CharScript cs)
    {
        if (cs.playableGraph.IsValid())
        {
            for (int i = 0; i < cs.playableGraph.GetRootPlayableCount(); i++)
            {
                var playable = cs.playableGraph.GetRootPlayable(i);
                if (playable.GetPlayState() == PlayState.Paused)
                {
                    playable.Play();
                }
                else
                {
                    playable.Pause();
                }
            }
        }
    }
    public void PauseAll()
    {
        var cs_array = GameObject.FindObjectsOfType<CharScript>();
        foreach (var cs in cs_array)
        {
           PauseCharacter(cs);
        }
    }
    public void ResumeCharacter(CharScript cs)
    {
        if (cs.playableGraph.IsValid())
        {
            for (int i = 0; i < cs.playableGraph.GetRootPlayableCount(); i++)
            {
                var playable = cs.playableGraph.GetRootPlayable(i);

                if (playable.GetPlayState() == PlayState.Paused)
                {
                    playable.Play();
                }

            }
        }
    }
    public void ResumeAll()
    {
        var cs_array = GameObject.FindObjectsOfType<CharScript>();
        foreach (var cs in cs_array)
        {
            ResumeCharacter(cs);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
