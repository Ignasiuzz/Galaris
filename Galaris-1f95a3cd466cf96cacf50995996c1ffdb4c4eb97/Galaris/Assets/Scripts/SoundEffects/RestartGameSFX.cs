using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGameSFX : MonoBehaviour
{
    public AudioSource SFX;
    public AudioClip ButtonClick;

    public void Button1()
    {
    SFX.clip = ButtonClick;
    SFX.Play();
    }

    public void Button2()
    {
    SFX.clip = ButtonClick;
    SFX.Play();
    }
}
