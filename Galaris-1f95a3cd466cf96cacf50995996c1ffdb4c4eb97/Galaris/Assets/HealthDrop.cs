using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class HealthPickup : MonoBehaviour
{
    Player playerHealth;
    Health HP;

    public float addHealth;

    void Awake(){
        playerHealth = FindObjectOfType<Player>();
        HP = FindObjectOfType<Health>();
    }

    void OnTriggerEnter2D(Collider2D col){
        addHealth = playerHealth.maxHealth / 10;

        if (playerHealth.currentHealth < playerHealth.maxHealth){

            Destroy(gameObject);
            Debug.Log("Health Picked Up!");
            playerHealth.currentHealth = playerHealth.currentHealth + addHealth;
            HP.UpdateHealthBar(playerHealth.currentHealth, playerHealth.maxHealth);
            HP.SetHealth(playerHealth.currentHealth);

        }
    }
}