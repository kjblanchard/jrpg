using UnityEngine;
using DG.Tweening;

public class GetBig : StatusEffect
{
    private const byte _statusTurnLength = 4;
    public GetBig(Battler battler) : base(battler, _statusTurnLength)
    {
        StatusEffectName = StatusEffectList.GetBig;
        StatusEffectStatModifiers.Str = 3;
    }
    public override void OnFirstApplied()
    {
        base.OnFirstApplied();
        var currentScale = _battlerToReference.spriteComp.transform.localScale;
        var newScale = currentScale * 1.25f;
        var tween = _battlerToReference.spriteComp.transform.DOScale(newScale, 1.5f);
        tween.SetEase(Ease.InOutQuint);
        DOTween.Play(tween);

        StatusEffectEndEvent += effect =>
        {
            var backTween = _battlerToReference.spriteComp.transform.DOScale(currentScale, 1.0f);
            tween.SetEase(Ease.InOutQuint);
            DOTween.Play(backTween);
            SoundController.Instance.PlaySfx(SoundController.Sfx.GetSmall);
        };



    }

}
