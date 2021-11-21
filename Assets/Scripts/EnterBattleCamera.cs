using DG.Tweening;
using UnityEngine;

public class EnterBattleCamera : MonoBehaviour
{
    [SerializeField] private MainCamera _mainCamera;
    [SerializeField] private Animator _blackBarAnimation;
    [SerializeField] private const string _blackBarTriggerString = "start";

    /// <summary>
    /// Starts the regular battle transition
    /// </summary>
    public void StartBattleTransition()
    {
        StartAllTweens();
        _blackBarAnimation.SetTrigger(_blackBarTriggerString);
    }

    private void StartAllTweens()
    {
        DOTween.Play("cameraZoomIn");
        DOTween.Play("cameraRotate");
        DOTween.Play("tintCamera");
    }


}
