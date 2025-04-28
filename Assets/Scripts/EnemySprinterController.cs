using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprinterController : MonoBehaviour
{
    private float speed = 6.0f;
    public Transform target;
    private float health = 3;
    public GameObject enemy;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        target = GameObject.FindGameObjectWithTag("Hive").GetComponent<Transform>();
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.0f);
        Vector2 direction = target.position - transform.position;
        direction.Normalize();

        if (health <= 0)
        {
            Destroy(enemy);
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
}
