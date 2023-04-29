using DG.Tweening;
using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingOption : MonoBehaviour
{
    [SerializeField] BoolReference variable;
    [SerializeField] private UnityEvent onTurnedOn, onTurnedOff;
    [SerializeField] private Image toggleBack;
    [SerializeField] private RectTransform toggle;
    [SerializeField] private float moveDistanceFromCenter = 45;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color disabledColor;

    private bool open = true;

    private void Awake()
    {
        open = variable.Value;
        UpdateOption();
    }

    public void Toggle()
    {
        open = !open;
        variable.Value = open;
        UpdateOption();
    }

    private void UpdateOption()
    {
        if (open)
        {
            toggle.transform.localPosition = Vector3.right * moveDistanceFromCenter;
            toggleBack.color = activeColor;
            onTurnedOn.Invoke();
        }
        else
        {
            toggle.transform.localPosition = Vector3.left * moveDistanceFromCenter;
            toggleBack.color = disabledColor;
            onTurnedOff.Invoke();
        }
    }
}
