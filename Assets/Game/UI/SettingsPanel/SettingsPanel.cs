using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : FateMonoBehaviour
{
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private Canvas option1Canvas = null;
    [SerializeField] private Canvas option2Canvas = null;

    public void Open()
    {
        GameManager.Instance.PlayHaptic();
        GameManager.Instance.PauseGame();
        canvas.enabled = true;
        option1Canvas.enabled = true;
        option2Canvas.enabled = true;
    }

    public void Close()
    {
        GameManager.Instance.PlayHaptic();
        GameManager.Instance.ResumeGame();
        canvas.enabled = false;
        option1Canvas.enabled = false;
        option2Canvas.enabled = false;
    }
}
