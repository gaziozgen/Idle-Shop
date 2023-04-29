using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalBubbleManager : FateMonoBehaviour
{
    static public PersonalBubbleManager Instance { get; private set; }

    private Camera mainCamera = null;
    private List<PersonalBubble> bubbleList = new List<PersonalBubble>();

    private void Awake()
    {
        mainCamera = Camera.main;
        Instance = this;
    }

    public void AddBubble(PersonalBubble bubble)
    {
        bubbleList.Add(bubble);
    }

    void Update()
    {
        Vector3 rotation = mainCamera.transform.rotation.eulerAngles;
        for (int i = 0; i < bubbleList.Count; i++)
        {

            bubbleList[i].UpdateByManager(rotation);
        }
    }
}
