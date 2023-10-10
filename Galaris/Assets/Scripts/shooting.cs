using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;

    private PlayableArea playableArea; // Reference to the PlayableArea script.

    private void Start()
    {
        // Find the PlayableArea GameObject in the scene.
        playableArea = GameObject.FindObjectOfType<PlayableArea>();
    }

    // Update is called once per frame
    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        // Start a coroutine to check if the bullet is outside the playable area.
        StartCoroutine(CheckBulletOutOfBounds(bullet));
    }

    // Coroutine to check if the bullet is outside the playable area.
    IEnumerator CheckBulletOutOfBounds(GameObject bullet)
    {
        while (true)
        {
            // Check if the bullet's position is outside the playable area.
            if (bullet.transform.position.x < -playableArea.width / 2f ||
                bullet.transform.position.x > playableArea.width / 2f ||
                bullet.transform.position.y < -playableArea.height / 2f ||
                bullet.transform.position.y > playableArea.height / 2f)
            {
                // Bullet is outside the playable area; destroy it.
                Destroy(bullet);
                yield break; // Exit the coroutine.
            }

            yield return null; // Wait for the next frame to continue checking.
        }
    }
}