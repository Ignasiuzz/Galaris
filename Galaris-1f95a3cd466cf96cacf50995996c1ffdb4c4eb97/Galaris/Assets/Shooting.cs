using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shooting : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject PlayerBullet;

    public float bulletForce = 30f; //kulkos greitis

    public void Shoot()
    {
        GameObject bullet = Instantiate(PlayerBullet, FirePoint.position, FirePoint.rotation);//Sukuri kulka, kuria saus
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(FirePoint.right * bulletForce, ForceMode2D.Impulse);//pasiuncia ta kulka tiesiai

    }
}