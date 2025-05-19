using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public GameObject bullet;
    private float speed = 20f;
    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public static float bulletSpread = 1.2f;
    public Transform player;

    public Vector3? overrideDirection = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        if (overrideDirection.HasValue)
        {
            Vector3 dir = overrideDirection.Value;
            rb.velocity = dir.normalized * speed;
        }
        else
        {
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 kierunek = mousePos - player.transform.position;
            rb.velocity = new Vector2(
                kierunek.x + Random.Range(-bulletSpread, bulletSpread),
                kierunek.y + Random.Range(-bulletSpread, bulletSpread)
            ).normalized * speed;
        }
    }

    void Update()
    {
        Destroy(bullet, 0.8f);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Environment"))
        {
            Destroy(bullet);
        }
    }
}
