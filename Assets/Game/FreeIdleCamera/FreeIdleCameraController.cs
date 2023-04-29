using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using DG.Tweening;

public class FreeIdleCameraController : FateMonoBehaviour
{
    public static FreeIdleCameraController Instance { get; private set; }
    [SerializeField] private Swerve swerve = null;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector4 firstClampRanges = Vector4.zero;
    [SerializeField] private Vector4 secondClampRanges = Vector4.zero;

    private Vector3 anchorPosition = Vector3.zero;
    private bool clampUpgraded = false;
    private Vector4 currentClampRanges;

    private bool free = true;

    private void Awake()
    {
        Instance = this;
        currentClampRanges = firstClampRanges;
        swerve.OnStart.AddListener(OnStart);
        swerve.OnSwerve.AddListener(OnSwerve);
        swerve.OnRelease.AddListener(OnRelease);
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
        transform.DOKill();
        transform.DOMove(pos, duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            free = true;
            OnStart();
        });
    }

    private void OnStart()
    {
        anchorPosition = transform.position;
    }

    private void OnSwerve()
    {
        if (!free) return;
        Vector3 moveOnXZ = ((Vector3.back + Vector3.right).normalized * swerve.XRate
            + (Vector3.forward + Vector3.right).normalized * swerve.YRate) * speed;

        Vector3 targetPos = anchorPosition - moveOnXZ;
        transform.position = Vector3.right * Mathf.Clamp(targetPos.x, currentClampRanges.w, currentClampRanges.y) 
            + Vector3.forward * Mathf.Clamp(targetPos.z, currentClampRanges.z, currentClampRanges.x);
    }

    private void OnRelease()
    {

    }

}
