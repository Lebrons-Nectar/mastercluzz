using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public static float playerSpeed = 4.0f;
    public static int playerHealth = 1;

    public bool isAlive = true;
    public SpriteRenderer spriteRenderer;

    [SerializeField] private Animator _animator;

    private UIManager uiManager;

    void Start()
    {
        playerHealth = 1;
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.0f);

        // Find UIManager once at start
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager not found in scene!");
        }
        
        // Auto-find animator if missing
        if (!_animator) _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isAlive)
        {
            // Reverted to GetAxisRaw for snappy movement (no skiing)
            Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            
            // Normalize to prevent faster diagonal movement
            if (playerInput.magnitude > 1f) playerInput.Normalize();
            
            transform.position += playerInput * playerSpeed * Time.deltaTime;

            // Adjust Animation Speed: Slow down when idle, normal when running
            if (playerInput.sqrMagnitude == 0)
            {
                _animator.speed = 0.5f; // Slower idle animation
            }
            else
            {
                _animator.speed = 1f;   // Normal running speed
            }

            // Animation Priority: Vertical > Horizontal
            // If moving vertically, ignore horizontal animations so "Up/Down" overrides "Left/Right"
            if (Mathf.Abs(playerInput.y) > 0.1f)
            {
                _animator.SetBool("runningW", playerInput.y > 0.1f);
                _animator.SetBool("runningS", playerInput.y < -0.1f);
                _animator.SetBool("runningA", false);
                _animator.SetBool("runningD", false);
            }
            else
            {
                _animator.SetBool("runningW", false);
                _animator.SetBool("runningS", false);
                _animator.SetBool("runningA", playerInput.x < -0.1f);
                _animator.SetBool("runningD", playerInput.x > 0.1f);
            }
        }

        if (playerHealth <= 0 && isAlive)
        {
            Die();
        }

    }

    private void Die()
    {
        Debug.Log("💀 Player has died.");
        isAlive = false;

        if (uiManager != null)
        {
            uiManager.ShowDeathScreen();
        }
        else
        {
            Debug.LogError("❌ uiManager reference missing during death!");
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    // Call this function to deal damage
    public void TakeDamage(int amount)
    {
        playerHealth -= amount;
        Debug.Log("💥 Player took " + amount + " damage. Health: " + playerHealth);
    }
}
