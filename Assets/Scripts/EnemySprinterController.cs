using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprinterController : MonoBehaviour
{
    private float speed = 6.0f;
    private float currentSpeed;
    public Transform target;
    private float health = 3;
    public GameObject enemy;
    public WaveSpawner WS;

    void Start()
    {
        WS = GameObject.FindGameObjectWithTag("WaveLogic").GetComponent<WaveSpawner>();
        currentSpeed = speed;
    }

    void Update()
    {
        target = GameObject.FindGameObjectWithTag("Hive").GetComponent<Transform>();
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, currentSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.0f);
        Vector2 direction = target.position - transform.position;
        direction.Normalize();

        if (health <= 0)
        {
            Destroy(enemy);
            WS.GreenFuckNiggers();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterMovement.playerHealth--;
        }

        if (other.CompareTag("Bullet"))
        {
            health--;
        }
    }

    public void SetSlowed(float slowMultiplier)
    {
        currentSpeed = speed * slowMultiplier;
    }

    public void RemoveSlow()
    {
        currentSpeed = speed;
    }
}
