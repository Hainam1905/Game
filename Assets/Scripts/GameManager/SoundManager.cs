using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip soundGenerateObject, soundClick, soundWin;

    public bool isEnable = true;

    [Range(0, 1)] [SerializeField]
    private float volumne = 1f;

    public float Volumne { get => volumne; set => volumne = Mathf.Clamp01(value); }
    public AudioClip SoundGenerateObject { get => soundGenerateObject; }
    public AudioClip SoundClick { get => soundClick; }
    public AudioClip SoundWin { get => soundWin; }

    private void Start()
    {
        if (!SoundGenerateObject || !SoundClick || !SoundWin)
        {
            Debug.Log("ERROR!!! some sound is missing");
        }
    }

    public void PlaySound(AudioClip audioClip, float volumne)
    {
        if (audioClip && isEnable)
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, this.Volumne * Mathf.Clamp(volumne, 0.05f, 1));
    }

    public void PlayClickSound()
    {
        if (SoundClick && isEnable)
            AudioSource.PlayClipAtPoint(SoundClick, Camera.main.transform.position, this.Volumne);
    }
}
