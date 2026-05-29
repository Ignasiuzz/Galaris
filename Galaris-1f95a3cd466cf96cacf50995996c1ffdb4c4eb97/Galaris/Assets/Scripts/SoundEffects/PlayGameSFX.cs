using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameSFX : MonoBehaviour
{
    public AudioSource SFX;
    public AudioClip ButtonClick;

    public void Button1()
    {
    SFX.clip = ButtonClick;
    SfxLimiter.TryPlay(SFX, "ui_button", 0.08f, 1, 0.08f);
    }
}
