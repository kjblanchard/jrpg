using System.Linq.Expressions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidBody;
    [SerializeField]
    private PlayerInteractionComponent _playerInteractionComponent;
    [SerializeField]
    private PlayerBattleComponent _playerBattleComponent;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.targetFrameRate = 60;
        }

    }
    /// <summary>
    /// Handles the mouseclick using unitys new input system.
    /// </summary>
    private void OnMouseClick()
    {
        if (_playerInteractionComponent.CurrentlyInteracting || _playerBattleComponent.InBattle)
            return;

        var positionToMoveTo = CalculatePositionToMoveForAllBuilds(Application.platform);
        _rigidBody.gameObject.transform.DOMove(positionToMoveTo, 1.0f);
        // _rigidBody.DOPlay();
        _playerBattleComponent.UpdateCurrentBattleAreaStepCounter();
    }

    private Vector2 CalculatePositionToMoveForAllBuilds(RuntimePlatform runtimeToLookFor)
    {
        var positionOfClickOrTouch = runtimeToLookFor switch
        {
            RuntimePlatform.Android => Touchscreen.current.primaryTouch.position.ReadValue(),
            RuntimePlatform.IPhonePlayer => Touchscreen.current.primaryTouch.position.ReadValue(),
            _ => Mouse.current.position.ReadValue()
        };
        var screenPos = Camera.main.ScreenToWorldPoint(positionOfClickOrTouch);
        return new Vector2(screenPos.x, screenPos.y);
    }


    /// <summary>
    /// Handles the Right mouseclick using unitys new input system.
    /// </summary>
    private void OnMouseRightClick()
    {
        _playerInteractionComponent.PerformInteraction();
    }

}
