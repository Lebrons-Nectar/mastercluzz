using UnityEngine;

public class TurretObject : MonoBehaviour
{
    [SerializeField] private float range = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private float fireCooldown;

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        GameObject target = FindNearestEnemy();
        if (target == null) return;

        if (fireCooldown <= 0f)
        {
            FireAtTarget(target.transform.position);
            fireCooldown = 1f / fireRate;
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= range)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    private void FireAtTarget(Vector3 targetPosition)
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector3 direction = (targetPosition - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        if (bulletScript != null)
        {
            bulletScript.overrideDirection = direction;
        }
    }
}
