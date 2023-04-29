using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : FateMonoBehaviour
{
    [SerializeField] private Canvas canvas = null;

    public void Open()
    {
        GameManager.Instance.PauseGame();
        canvas.enabled = true;
    }

    public void Close()
    {
        GameManager.Instance.ResumeGame();
        canvas.enabled = false;
    }
}
