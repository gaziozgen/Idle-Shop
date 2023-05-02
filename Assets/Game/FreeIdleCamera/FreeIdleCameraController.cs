using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using DG.Tweening;
using UnityEngine.InputSystem.LowLevel;

public class FreeIdleCameraController : FateMonoBehaviour
{
    private static FreeIdleCameraController instance = null;
    public static FreeIdleCameraController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<FreeIdleCameraController>();
            return instance;
        }
    }
    [SerializeField] private Camera _camera = null;
    [SerializeField] private GameStateVariable gameState = null;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float zoomOutMin = 8;
    [SerializeField] private float zoomOutMax = 20;
    [SerializeField] private float driftAcceleration = 1f;
    [SerializeField] private Vector4 firstClampRanges = Vector4.zero;
    [SerializeField] private Vector4 secondClampRanges = Vector4.zero;

    private Vector4 currentClampRanges;
    private bool clampUpgraded = false;
    private bool free = true;
    private int lastTouchCount = 0;

    private Vector3 anchorPosition = Vector3.zero;
    private bool swerveOpenedOnWrongTime = false;

    private bool drift = false;
    private bool cancelNextDrift = false;
    private Vector3 startDriftSpeed = Vector3.zero;
    private float driftStartTime = 0f;
    private float totalDriftTime = 0f;
    private Vector3 lastPosition = Vector3.zero;

    private void Awake()
    {
        currentClampRanges = firstClampRanges;
    }

    public void UpgradeClamp()
    {
        if (!clampUpgraded)
        {
            clampUpgraded = true;
            currentClampRanges = secondClampRanges;
        }
        else Debug.LogError("Yanlýþ koþulda çaðýrýlmýþ");
    }

    public void Focus(Vector3 pos, float duration)
    {
        free = false;
        drift = false;
        transform.DOKill();
        transform.DOMove(pos, duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            free = true;
            UpdateAnchorAndCloseDrift();
        });
    }

    private void LateUpdate()
    {
        if (gameState.Value == GameState.PAUSED || !free)
        {
            //print("gameState.Value == GameState.PAUSED");
            if (Input.touchCount == 1) swerveOpenedOnWrongTime = true;
        }
        else
        {
            // ########################################################################################################
            //MobileControl(); 
            PcControl();

            if (drift)
            {
                Vector3 targetPos = transform.position + startDriftSpeed * Time.deltaTime * (1 - (Time.time - driftStartTime) / totalDriftTime);
                Clamp(targetPos);
                if (Time.time > driftStartTime + totalDriftTime) drift = false;
            }


            Zoom(Input.GetAxis("Mouse ScrollWheel") * 10);
        }

        lastTouchCount = Input.touchCount;
    }

    private void MobileControl()
    {
        if (Input.touchCount == 1 && lastTouchCount == 0)
        {
            //print("UpdateAnchorAndCloseDrift");
            UpdateAnchorAndCloseDrift();
        }
        else if (Input.touchCount == 1 && lastTouchCount == 1)
        {
            //print("OnSwerve");
            OnSwerve();
        }
        else if (Input.touchCount == 0 && lastTouchCount == 1)
        {
            //print("OnRelease");
            OnRelease();
        }
        else if (Input.touchCount == 2 && lastTouchCount == 1)
        {
            //print("cancelNextDrift = true");
            cancelNextDrift = true;
        }
        else if (Input.touchCount == 2 && lastTouchCount == 2)
        {
            //print("ZoomByTwoFinger");
            ZoomByTwoFinger();
        }
        else if (Input.touchCount == 1 && lastTouchCount == 2)
        {
            //print("anchorPosition = _camera.ScreenToWorldPoint(Input.mousePosition)");
            anchorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void PcControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdateAnchorAndCloseDrift();
        }
        if (Input.GetMouseButton(0))
        {
            Move();
        }
    }

    private void UpdateAnchorAndCloseDrift()
    {
        drift = false;
        anchorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void ZoomByTwoFinger()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
        float difference = currentMagnitude - prevMagnitude;
        Zoom(difference * 0.01f);
    }

    private void OnSwerve()
    {
        lastPosition = transform.position;
        if (swerveOpenedOnWrongTime) return;

        Move();

        /*Vector3 moveOnXZ = (Vector3.back + Vector3.right).normalized * swerve.XRate * speedX
            + (Vector3.forward + Vector3.right).normalized * swerve.YRate * speedY;

        Vector3 targetPos = anchorPosition - moveOnXZ;*/
    }

    private void OnRelease()
    {
        if (cancelNextDrift || swerveOpenedOnWrongTime)
        {
            if (cancelNextDrift) cancelNextDrift = false;
            if (swerveOpenedOnWrongTime) swerveOpenedOnWrongTime = false;
            return;
        }

        startDriftSpeed = (transform.position - lastPosition) / Time.deltaTime;
        if (startDriftSpeed == Vector3.zero) return;

        drift = true;
        totalDriftTime = startDriftSpeed.magnitude / driftAcceleration;
        driftStartTime = Time.time;
    }

    private void Move()
    {
        Vector3 direction = anchorPosition - _camera.ScreenToWorldPoint(Input.mousePosition);
        direction.y = 0;
        Vector3 targetPos = transform.transform.position + direction;
        Clamp(targetPos);
    }

    private void Zoom(float increment)
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - increment * zoomSpeed, zoomOutMin, zoomOutMax);
    }

    private void Clamp(Vector3 targetPos)
    {
        transform.position = Vector3.right * Mathf.Clamp(targetPos.x, currentClampRanges.w, currentClampRanges.y)
            + Vector3.forward * Mathf.Clamp(targetPos.z, currentClampRanges.z, currentClampRanges.x);
    }

}
