using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{
    
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float bulletforce = 20f;



    // Update is called once per frame

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.AddForce(firePoint.up * bulletforce, ForceMode2D.Impulse);
    }
}
