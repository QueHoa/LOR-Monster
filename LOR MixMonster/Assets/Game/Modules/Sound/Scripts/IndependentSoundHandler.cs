using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndependentSoundHandler : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private bool playOnAwake = true;

    private void OnEnable()
    {
        if(playOnAwake && Sound.Controller.SfxEnable)
        {
            Play();
        }
    }
    public void Play()
    {
        audioSource.Play();

    }
    public void Stop()
    {
        audioSource.Stop();
    }
}
