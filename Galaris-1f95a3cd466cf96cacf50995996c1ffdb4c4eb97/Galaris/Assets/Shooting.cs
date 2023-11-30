using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shooting : MonoBehaviour
{
    public Transform killME;
    public GameObject PlayerBullet;

    public float bulletForce = 30f; //kulkos greitis

    public void Shoot()
    {
        GameObject bullet = Instantiate(PlayerBullet, killME.position, killME.rotation);//Sukuri kulka, kuria saus
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(killME.right * bulletForce, ForceMode2D.Impulse);//pasiuncia ta kulka tiesiai

    }
}