using UnityEngine;

public class LogicManager : MonoBehaviour
{
    private PlayerValues playerValues;

    void Start()
    {
        playerValues = GetComponent<PlayerValues>();
        if (playerValues == null)
        {
            Debug.LogError("❌ LogicManager couldn't find PlayerValues on the same GameObject!");
        }
    }

    public void EndWave()
    {
        Debug.Log("✅ Wave ended! Awarding income...");
        if (playerValues != null)
        {
            playerValues.AddIncome();
        }
    }
}
