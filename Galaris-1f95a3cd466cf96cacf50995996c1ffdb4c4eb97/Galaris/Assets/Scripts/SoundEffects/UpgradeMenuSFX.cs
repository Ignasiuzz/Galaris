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
    SfxLimiter.TryPlay(SFX, "ui_menu", 0.1f, 1, 0.1f);
    }

    public void UpgradeButton()
    {
    SFX.clip = PurchaseUpgrade;
    SfxLimiter.TryPlay(SFX, "ui_purchase", 0.08f, 2, 0.15f);
    }
}
