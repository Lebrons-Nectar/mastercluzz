using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public static float playerSpeed = 3.3f;
    public bool isAlive = true;
    public SpriteRenderer spriteRenderer;
    public static int playerHealth = 3;
    public GameObject player;

    void Start()
    {
       // logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<logicScript>();
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
           // logic.gameOver();
        }
    }
}
