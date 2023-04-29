using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FateGames.Core
{
    public class Swerve : MonoBehaviour
    {
        [SerializeField] protected int Size;
        public Vector2 AnchorPosition { get; protected set; } = Vector2.zero;
        public Vector2 MousePosition { get; protected set; } = Vector2.zero;
        public Vector2 Difference { get => MousePosition - AnchorPosition; }
        public float Distance { get => Difference.magnitude; }
        public float Rate { get => Distance / Size; }
        public float XRate { get => Difference.x / Size; }
        public float YRate { get => Difference.y / Size; }
        public readonly UnityEvent OnStart = new();
        public readonly UnityEvent OnSwerve = new();
        public readonly UnityEvent OnRelease = new();


        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) OnMouseButtonDown();
            else if (Input.GetMouseButton(0)) OnMouseButton();
            else if (Input.GetMouseButtonUp(0)) OnMouseButtonUp();
        }


        protected virtual void OnMouseButtonDown()
        {
            //print("Touch Down");
            MousePosition = Input.mousePosition;
            AnchorPosition = MousePosition;
            OnStart.Invoke();
        }

        protected virtual void OnMouseButton()
        {
            //print("Touch");
            Vector2 mousePosition = Input.mousePosition;
            Vector2 direction = (mousePosition - AnchorPosition).normalized;
            MousePosition = AnchorPosition + direction * Mathf.Clamp((mousePosition - AnchorPosition).magnitude, 0, Size);
            OnSwerve.Invoke();
        }

        protected virtual void OnMouseButtonUp()
        {
            //print("Touch Up");
            OnRelease.Invoke();
        }

    }
}

