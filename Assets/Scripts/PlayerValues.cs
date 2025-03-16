using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hive
{
    // TBP = TO BE PLACED P = PLACED
    public int HiveTBPCount;
    public int HivePCount;
    public int Income;
    public int IncomePerWave(int HivesPlaced, int IncomePerHive)
    {
        int income = HivesPlaced * IncomePerHive;
        return income;
    }
}

public class PlayerValues : MonoBehaviour
{
    [SerializeField] public int Honey;
    [SerializeField] public Text HoneyCounter;

    public int IncomeTotal(int a, int b, int c)
    {
        int x = a + b + c;
        return x;
    }
    public Hive Tier1Hive = new Hive();
    public Hive Tier2Hive = new Hive();
    public Hive Tier3Hive = new Hive();

    void Start()
    {
        Tier1Hive.Income = 100;
        Tier2Hive.Income = 200;
        Tier3Hive.Income = 300;
    }

    void Update()
    {
        HoneyCounter.text = Honey.ToString();
    }

    public void AddIncome()
    {
        Honey += IncomeTotal(Tier1Hive.IncomePerWave(Tier1Hive.HivePCount, Tier1Hive.Income), Tier2Hive.IncomePerWave(Tier2Hive.HivePCount, Tier2Hive.Income), Tier3Hive.IncomePerWave(Tier3Hive.HivePCount, Tier3Hive.Income));
    }
}
