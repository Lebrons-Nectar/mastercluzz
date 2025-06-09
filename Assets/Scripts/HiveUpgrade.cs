using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiveUpgrade : MonoBehaviour
{
    private int hiveTier = 1;
    [SerializeField] private GameObject upgradeText;
    [SerializeField] private SpriteRenderer hiveSprite;
    public PlayerValues PlayerVal;
    private bool inRange = false;

    [SerializeField] private Sprite hiveTier2sprite;
    [SerializeField] private Sprite hiveTier3sprite;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            upgradeText.SetActive(true);
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            upgradeText.SetActive(false);
            inRange = false;
        }
    }
    private void Start()
    {
        PlayerVal = GameObject.FindGameObjectWithTag("Logic").GetComponent<PlayerValues>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inRange)
        {
            switch (hiveTier)
            {
                case 1:
                    if(PlayerVal.Honey >= 2000)
                    {
                        hiveTier++;
                        PlayerVal.Honey -= 2000;
                        hiveSprite.sprite = hiveTier2sprite;
                        PlayerVal.Tier1Hive.HivePCount--;
                        PlayerVal.Tier2Hive.HivePCount++;
                    }
                    break;
                case 2:

                    break;
            }
        }
    }
}
