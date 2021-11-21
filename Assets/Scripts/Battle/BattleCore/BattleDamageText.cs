using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BattleDamageText : MonoBehaviour
{
    [SerializeField]
    private DOTweenAnimation moveAnim;

    [SerializeField] private DOTweenAnimation scaleAnim;
    [SerializeField] private DOTweenAnimation fadeAnim;

    [SerializeField] private DotweenBroadcasterComponent _fadeBroadcasterComponent;
    [SerializeField] private TMP_Text textToDisplay;

    private void Awake()
    {
        _fadeBroadcasterComponent.DotweenCompleteEvent += OnDamageFadeComplete;
    }
    public void PlayDamage(string displayText, Color colorToDisplay)
    {
        textToDisplay.text = displayText;
        textToDisplay.color = colorToDisplay;
        scaleAnim.DORestart();
        fadeAnim.DORestart();
        moveAnim.DORestart();


    }

    public void OnDamageFadeComplete(object obj, EventArgs e)
    {
        scaleAnim.DORewind();
        fadeAnim.DORewind();
        moveAnim.DORewind();
        PutBackInQueue();
    }

    public Action PutBackInQueue;




}
