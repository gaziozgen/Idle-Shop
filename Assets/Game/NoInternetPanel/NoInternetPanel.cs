
using FateGames.Core;
using UnityEngine;

public class NoInternetPanel : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        InvokeRepeating(nameof(Check), 0, 0.2f);
    }
    private void Check()
    {
        if (canvas.enabled) return;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            GameManager.Instance.PauseGame();
            canvas.enabled = true;
        }
    }
    public void Retry()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            GameManager.Instance.ResumeGame();
            canvas.enabled = false;
        }
    }
}
