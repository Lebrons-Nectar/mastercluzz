using UnityEngine;

public class BarbedWireTrap : MonoBehaviour
{
    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private float damagePerSecond = 5f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController ec = other.GetComponent<EnemyController>();
            EnemySprinterController esc = other.GetComponent<EnemySprinterController>();

            if (ec != null)
            {
                ec.SetSlowed(slowMultiplier);
                ec.SendMessage("TakeDamage", damagePerSecond * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
            }

            if (esc != null)
            {
                esc.SetSlowed(slowMultiplier);
                esc.SendMessage("TakeDamage", damagePerSecond * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController ec = other.GetComponent<EnemyController>();
            EnemySprinterController esc = other.GetComponent<EnemySprinterController>();

            if (ec != null)
            {
                ec.RemoveSlow();
            }

            if (esc != null)
            {
                esc.RemoveSlow();
            }
        }
    }
}
