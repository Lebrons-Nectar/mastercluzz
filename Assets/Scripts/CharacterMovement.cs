using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public static float playerSpeed = 4.0f;
    public bool isAlive = true;
    public SpriteRenderer spriteRenderer;
    [SerializeField] public static int playerHealth = 1;
    public GameObject player;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.0f);
        playerHealth = 3;
    }

    void Update()
    {
        if (isAlive == true)
        {
            Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            transform.position = transform.position + playerInput.normalized * playerSpeed * Time.deltaTime;
        }

        if (playerHealth <= 0)
        {
            isAlive = false;
            Destroy(player);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            playerHealth--;
        }
    }
}
