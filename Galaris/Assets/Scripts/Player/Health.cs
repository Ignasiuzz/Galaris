using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    //public Slider slider;
    [SerializeField] private Slider slyder;
    public void UpdateHealthbar(float currentvalue, float maxvalue)
    {
        slyder.value = currentvalue / maxvalue;
    }

   public void SetMaxHealth(int health)
   { 
       slyder.maxValue = health;
       slyder.value = health;
}

    public void SetHealth(int health)
    {
       slyder.value = health;
    }
}
