using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicManager : MonoBehaviour
{

    public PlayerValues PlayerVal;
    public Text WaveCounterT;
    public WaveSpawner WaveSpawner;
    void Start()
    {
        WaveSpawner = GameObject.FindGameObjectWithTag("Logic").GetComponent<WaveSpawner>();
    }

    void Update()
    {
        WaveCounterT.text = WaveSpawner.currWave.ToString();
    }

    public void EndWave()
    {
        PlayerVal.AddIncome();
    }
}

