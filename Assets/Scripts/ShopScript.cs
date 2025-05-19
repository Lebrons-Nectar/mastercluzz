using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    public PlayerValues PlayerVal;

    void Start()
    {
        PlayerVal = GameObject.FindGameObjectWithTag("Logic").GetComponent<PlayerValues>();
    }

    public void BuyTier1Hive()
    {
        if (PlayerVal.Honey >= 400)
        {
            PlayerVal.Honey -= 400;
            PlayerVal.Tier1Hive.HiveTBPCount++;
        }
    }

    public void BuyTier1Turret()
    {
        if (PlayerVal.Honey >= 600)
        {
            PlayerVal.Honey -= 600;
            PlayerVal.BasicTurret.TurretTBPCount++; // ✅ Correct field name
        }
    }
}
