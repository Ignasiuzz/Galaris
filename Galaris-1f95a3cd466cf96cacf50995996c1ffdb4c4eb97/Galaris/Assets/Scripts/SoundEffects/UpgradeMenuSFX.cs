using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenuSFX : MonoBehaviour
{
    public AudioSource SFX;
    public AudioClip UpgradeMenuOpen, PurchaseUpgrade;

    public void MenuButton()
    {
    SFX.clip = UpgradeMenuOpen;
    SFX.Play();
    }

    public void UpgradeButton()
    {
    SFX.clip = PurchaseUpgrade;
    SFX.Play();
    }
}
