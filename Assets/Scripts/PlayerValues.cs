using UnityEngine;
using UnityEngine.UI;

public class Hive
{
    public int HiveTBPCount;
    public int HivePCount;
    public int Income;

    public int IncomePerWave(int hivesPlaced, int incomePerHive)
    {
        return hivesPlaced * incomePerHive;
    }
}

public class TurretStats
{
    public int TurretTBPCount;
    public int TurretPCount;
}

public class PlayerValues : MonoBehaviour
{
    [Header("Economy")]
    public int Honey = 0;

    [Header("UI References")]
    public Text HoneyCounter;
    public Text Tier1HiveCounter;
    public Text TurretCountText;

    [Header("Hive Data")]
    public Hive Tier1Hive = new Hive();
    public Hive Tier2Hive = new Hive();
    public Hive Tier3Hive = new Hive();

    [Header("Turret Data")]
    public TurretStats BasicTurret = new TurretStats();

    private void Start()
    {
        // Hive Income Settings
        Tier1Hive.Income = 100;
        Tier2Hive.Income = 200;
        Tier3Hive.Income = 300;

        // Initial Hive and Turret Placement Values
        Tier1Hive.HiveTBPCount = 3;
        Tier1Hive.HivePCount = 0;

        BasicTurret.TurretTBPCount = 1;
        BasicTurret.TurretPCount = 0;
    }

    private void Update()
    {
        if (HoneyCounter != null)
            HoneyCounter.text = Honey.ToString();

        if (Tier1HiveCounter != null)
            Tier1HiveCounter.text = Tier1Hive.HiveTBPCount.ToString();

        if (TurretCountText != null)
            TurretCountText.text = BasicTurret.TurretTBPCount.ToString();
    }

    public void AddIncome()
    {
        int income =
            Tier1Hive.IncomePerWave(Tier1Hive.HivePCount, Tier1Hive.Income) +
            Tier2Hive.IncomePerWave(Tier2Hive.HivePCount, Tier2Hive.Income) +
            Tier3Hive.IncomePerWave(Tier3Hive.HivePCount, Tier3Hive.Income);

        Honey += income;
    }
}
