using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAimWeapon : MonoBehaviour
{
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
    }

    private Transform aimTransform;
    public Transform aimGunEndPointTransform;
    public GameObject Bullet;

    public int bulletCount;
    public bool isReloading = false;
    public Text ammoCount;
    // public Text Reloading; // Replaced by icon
    // public GameObject ReloadingText; // Replaced by icon
    public Image reloadingImage; // Assign in Inspector (Filled, Radial 360)

    public int maxAmmo = 17;  // BEDZIE ZDEFINIOWANE I ZMIENIAC SIE DLA KAZDEJ BRONI (PODANE SA DLA PISTOLETU)
    public float reloadTime = 2f;

   

    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;
    private AudioSource audioSource;

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        bulletCount = maxAmmo;
        audioSource = GetComponent<AudioSource>();
        
        // Ensure icon is visible and full at start
        if (reloadingImage)
        {
            reloadingImage.gameObject.SetActive(true);
            reloadingImage.fillAmount = 1f;
        }
    }

    void Update()
    {
        if(UIManager.isPaused == false)
        {
        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 aimDirection = (mousePosition - aimTransform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 a = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            a.y = -1f;
        }
        else
        {
            a.y = +1f;
        }
        aimTransform.localScale = a;
            
           
        HandleShooting();
        HandleReload();
        ammoCount.text = bulletCount.ToString();
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 10f; 
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && UIManager.isPaused == false && bulletCount > 0 && isReloading == false)
        {
            // Play Sound
            if (audioSource && shootSound)
            {
                audioSource.PlayOneShot(shootSound);
            }

            Instantiate(Bullet, aimGunEndPointTransform.position, transform.rotation);
            Vector3 mousePosition = GetMouseWorldPosition();
            bulletCount--;
            // ANIMACJA 
            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                gunEndPointPosition = aimGunEndPointTransform.position,
                 shootPosition = mousePosition, 
            }); 
        }
    }
    private void HandleReload()
    {
        if(Input.GetKeyDown(KeyCode.R) && !isReloading && bulletCount < maxAmmo)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        
        // Ensure it starts full for the "deplete" animation
        if (reloadingImage) reloadingImage.fillAmount = 1f;

        float timer = 0f;
        while (timer < reloadTime)
        {
            timer += Time.deltaTime;
            if (reloadingImage)
            {
                // Deplete clockwise: 1 -> 0
                reloadingImage.fillAmount = 1f - (timer / reloadTime);
            }
            yield return null;
        }

        // Snap back to full visibility ("Ready")
        if (reloadingImage) reloadingImage.fillAmount = 1f;

        bulletCount = maxAmmo;
        isReloading = false;
    }

}
