
using System;
using System.Drawing;
using UnityEngine;

public class BattlerLocationHandler
{
    private SpriteRenderer _battlerSpriteRenderer;
    private PointF _battlerSpriteRendererDimensions;
    private static Camera _mainCamera;

    private const float _nameDisplayXOffset = 0;
    private const float _nameDisplayYOffset = -0.05f;

    private const float _damageDisplayXOffset = -0.25f;
    private const float _damageDisplayYOffset = 0;

    private const float _mpDamageDisplayXOffset = 0.2f;
   private const float _mpDamageDisplayYOffset = -0.15f;

    private const float _damageDisplayXOffsetPlayer = 0.2f;
    private const float _damageDisplayYOffsetPlayer = -0.55f;

    private const float _mpDamageDisplayXOffsetPlayer = 0.4f;
    private const float _mpDamageDisplayYOffsetPlayer = -0.55f;

    public Vector3 GetNameDisplayInScreenPos => _mainCamera.WorldToScreenPoint(new Vector3(
        _battlerSpriteRenderer.transform.position.x + _nameDisplayXOffset,
        _battlerSpriteRenderer.transform.position.y - _battlerSpriteRendererDimensions.Y / 2 + _nameDisplayYOffset,
        _battlerSpriteRenderer.transform.position.z
        ));
    public Vector3 GetDamageDisplayInScreenPos => _mainCamera.WorldToScreenPoint(new Vector3(
        _battlerSpriteRenderer.transform.position.x + _battlerSpriteRendererDimensions.X / 2 + _damageDisplayXOffset,
        _battlerSpriteRenderer.transform.position.y + _battlerSpriteRendererDimensions.Y / 2 + _damageDisplayYOffset,
        _battlerSpriteRenderer.transform.position.z
        ));
    public Vector3 GetMpDamageDisplayInScreenPos => _mainCamera.WorldToScreenPoint(new Vector3(
        _battlerSpriteRenderer.transform.position.x + _battlerSpriteRendererDimensions.X / 2 + _mpDamageDisplayXOffset,
        _battlerSpriteRenderer.transform.position.y + _battlerSpriteRendererDimensions.Y / 2 + _mpDamageDisplayYOffset,
        _battlerSpriteRenderer.transform.position.z
        ));
    public Vector3 GetDamageDisplayInScreenPosPlayer => _mainCamera.WorldToScreenPoint(new Vector3(
        _battlerSpriteRenderer.transform.position.x - _battlerSpriteRendererDimensions.X / 2 + _damageDisplayXOffsetPlayer,
        _battlerSpriteRenderer.transform.position.y + _battlerSpriteRendererDimensions.Y / 2 + _damageDisplayYOffsetPlayer,
        _battlerSpriteRenderer.transform.position.z
        ));
    public Vector3 GetMpDamageDisplayInScreenPosPlayer => _mainCamera.WorldToScreenPoint(new Vector3(
        _battlerSpriteRenderer.transform.position.x - _battlerSpriteRendererDimensions.X / 2 + _mpDamageDisplayXOffsetPlayer,
        _battlerSpriteRenderer.transform.position.y + _battlerSpriteRendererDimensions.Y / 2 + _mpDamageDisplayYOffsetPlayer,
        _battlerSpriteRenderer.transform.position.z
        ));

    public BattlerLocationHandler(SpriteRenderer battlerSpriteRenderer)
    {
        _battlerSpriteRenderer = battlerSpriteRenderer;
        _battlerSpriteRendererDimensions =
            new PointF(_battlerSpriteRenderer.bounds.size.x, _battlerSpriteRenderer.bounds.size.y);
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    public Vector3 GetBattlerLocation(BattlerLocation theLocationToGrab)
    {

        return theLocationToGrab switch
        {
            BattlerLocation.Default => GetCenter,
            BattlerLocation.Top => GetTop,
            BattlerLocation.Bottom => GetBottom,
            BattlerLocation.Left => GetLeft,
            BattlerLocation.Right => GetRight,
            BattlerLocation.TopRight => Vector3.zero,
            BattlerLocation.BottomRight => Vector3.zero,
            BattlerLocation.LeftRight => Vector3.zero,
            _ => throw new ArgumentOutOfRangeException(nameof(theLocationToGrab), theLocationToGrab, null)
        };

    }

    public Vector3 GetBattlerHalfSpriteX => new Vector3(_battlerSpriteRendererDimensions.X / 2, 0, 0);
    public Vector3 GetBattlerHalfSpriteY => new Vector3(0, _battlerSpriteRendererDimensions.Y / 2, 0);

    private Vector3 GetCenter => _battlerSpriteRenderer.transform.position;
    private Vector3 GetTop => new Vector3(
        _battlerSpriteRenderer.transform.position.x,
        _battlerSpriteRenderer.transform.position.y + _battlerSpriteRendererDimensions.Y / 2,
        _battlerSpriteRenderer.transform.position.z
        );

    private Vector3 GetRight => new Vector3(
        _battlerSpriteRenderer.transform.position.x + _battlerSpriteRendererDimensions.X /2,
        _battlerSpriteRenderer.transform.position.y,
        _battlerSpriteRenderer.transform.position.z
    );
    private Vector3 GetLeft => new Vector3(
        _battlerSpriteRenderer.transform.position.x - _battlerSpriteRendererDimensions.X /2,
        _battlerSpriteRenderer.transform.position.y,
        _battlerSpriteRenderer.transform.position.z
    );

    private Vector3 GetBottom => _battlerSpriteRenderer.transform.position + GetBattlerHalfSpriteY;


    public enum BattlerLocation
    {
        Default,
        Top,
        Bottom,
        Left,
        Right,
        TopRight,
        BottomRight,
        LeftRight,
    }

}
