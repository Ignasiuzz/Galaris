using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class HealthPickup : MonoBehaviour
{
    Player playerHealth;

    public int addHealth = 10;

    void Awake(){
        playerHealth = FindObjectOfType<Player>();
    }

    void OnTriggerEnter2D(Collider2D col){
        if (playerHealth.currentHealth < playerHealth.maxHealth){

            Destroy(gameObject);
            Debug.Log("Health Picked Up!");
            playerHealth.currentHealth = playerHealth.currentHealth + addHealth;
            playerHealth.healthBar.UpdateHealthBar(playerHealth.currentHealth, playerHealth.maxHealth);

        }
    }
}