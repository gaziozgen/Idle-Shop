using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using UnityEngine.Events;

public class WaiterSpeedUpgradeController : MonoBehaviour
{
    [SerializeField] private FloatReference speedMultiplier;
    [SerializeField] private float increaseRatio = 1.1f;
    [SerializeField] private UnityEvent multiplierChanged;
    [SerializeField] private SaveDataVariable saveData;
    [SerializeField] private int maxSpeedLevel = 20;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        speedMultiplier.Value = Mathf.Pow(increaseRatio, saveData.Value.SpeedLevel);
        multiplierChanged.Invoke();
    }

    public bool IsUpgradeAvaliable()
    {
        if (saveData.Value.SpeedLevel < maxSpeedLevel) return true;
        else return false;
    }

    public void LevelUp()
    {
        saveData.Value.SpeedLevel++;
        speedMultiplier.Value = Mathf.Pow(increaseRatio, saveData.Value.SpeedLevel);
        multiplierChanged.Invoke();
    }
}
public partial class SaveData
{
    public int SpeedLevel = 0;
}
