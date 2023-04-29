using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickAnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button button;
    [SerializeField] private RectTransform container;
    [SerializeField] private float offset = 30;
    private Vector2 initialContainerPosition = Vector2.zero;

    private void Awake()
    {
        initialContainerPosition = container.anchoredPosition;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        container.anchoredPosition = initialContainerPosition + Vector2.down * offset;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        container.anchoredPosition = initialContainerPosition;
    }

}
