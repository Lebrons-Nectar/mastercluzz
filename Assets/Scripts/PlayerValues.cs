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
    public int Honey = 0;

    public Text HoneyCounter;
    public Text Tier1HiveCounter;
    public Text TurretCountText;
    public Text BarbedWireCounter;

    public Hive Tier1Hive = new Hive();
    public Hive Tier2Hive = new Hive();
    public Hive Tier3Hive = new Hive();

    public TurretStats BasicTurret = new TurretStats();
    public int BarbedWireCount = 0;

    private void Start()
    {
        Tier1Hive.Income = 100;
        Tier2Hive.Income = 200;
        Tier3Hive.Income = 300;

        Tier1Hive.HiveTBPCount = 3;
        Tier1Hive.HivePCount = 0;

        BasicTurret.TurretTBPCount = 1;
        BasicTurret.TurretPCount = 0;

        BarbedWireCount = 2;
    }

    private void Update()
    {
        if (HoneyCounter != null)
            HoneyCounter.text = Honey.ToString();

        if (Tier1HiveCounter != null)
            Tier1HiveCounter.text = Tier1Hive.HiveTBPCount.ToString();

        if (TurretCountText != null)
            TurretCountText.text = BasicTurret.TurretTBPCount.ToString();

        if (BarbedWireCounter != null)
            BarbedWireCounter.text = BarbedWireCount.ToString();
    }

    public void AddIncome()
    {
        int income =
            Tier1Hive.IncomePerWave(Tier1Hive.HivePCount, Tier1Hive.Income) +
            Tier2Hive.IncomePerWave(Tier2Hive.HivePCount, Tier2Hive.Income) +
            Tier3Hive.IncomePerWave(Tier3Hive.HivePCount, Tier3Hive.Income);

        Honey += income;
    }

    [ContextMenu("PIENIADZE")]
    public void CheatMoney()
    {
        Honey += 90000;
    }
}
