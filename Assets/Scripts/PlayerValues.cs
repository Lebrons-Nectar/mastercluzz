using UnityEngine;
using UnityEngine.UI;

public class Hive
{
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
    [SerializeField] public int Honey = 0;
    [SerializeField] public Text HoneyCounter;

    public Hive Tier1Hive = new Hive();
    public Hive Tier2Hive = new Hive();
    public Hive Tier3Hive = new Hive();

    void Start()
    {
        Tier1Hive.Income = 100;
        Tier2Hive.Income = 200;
        Tier3Hive.Income = 300;

        if (HoneyCounter == null)
        {
            Debug.LogError("❌ HoneyCounter is NOT assigned in the Inspector!");
        }
    }

    void Update()
    {
        if (HoneyCounter != null)
        {
            HoneyCounter.text = Honey.ToString();
        }
    }

    public int IncomeTotal(int a, int b, int c)
    {
        return a + b + c;
    }

    public void AddIncome()
    {
        Debug.Log("📦 AddIncome() called");

        int income = IncomeTotal(
            Tier1Hive.IncomePerWave(Tier1Hive.HivePCount, Tier1Hive.Income),
            Tier2Hive.IncomePerWave(Tier2Hive.HivePCount, Tier2Hive.Income),
            Tier3Hive.IncomePerWave(Tier3Hive.HivePCount, Tier3Hive.Income)
        );

        Honey += income;

        Debug.Log("🍯 Honey after income: " + Honey);
    }
}
