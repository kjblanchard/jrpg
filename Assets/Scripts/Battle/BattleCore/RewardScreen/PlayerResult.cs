using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResult : MonoBehaviour
{
    public bool IsLoaded = false;
    public bool IsFinished;
    public bool IsCurrentlyGainingExp => _expGainingCoroutine != null;

    [SerializeField] private DOTweenAnimation expBarColorTween;
    [SerializeField] private DOTweenAnimation expBarColorLevelUpTween;
    [SerializeField] private Image _playerImage;
    [SerializeField] private Image _playerPortraitColor;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _playerLevelText;
    [SerializeField] private TMP_Text _playerTotalExpText;
    [SerializeField] private TMP_Text _playerExpToNextLevelText;
    [SerializeField] private TMP_Text _playerExpGainText;
    [SerializeField] private Slider _playerExpSlider;

    private const float _defaultWaitTime = 0.1f;
    private const float _fastWaitTime = 0.001f;
    private const float _defaultLevelUpWaitTime = 1;
    private const float _fastLevelUpWaitTime = 0.1f;

    private static Color32 purpleColor = new Color32(192, 81, 160, 255);
    private BattleStats _playerBattleStats;
    private Coroutine _expGainingCoroutine;
    private int _expToGive;
    private WaitForSeconds defaultWaitTimeInstance = new WaitForSeconds(_defaultWaitTime);
    private WaitForSeconds defaultLevelUpInstance = new WaitForSeconds(_defaultLevelUpWaitTime);
    private bool _isInitiated;

    public void LoadBattlerIntoPlayerResult(BattleStats battlerToLoad, int expGained)
    {
        _playerBattleStats = battlerToLoad;
        _playerNameText.text = battlerToLoad.BattlerDisplayName;
        _playerLevelText.text = battlerToLoad.BattlerLvl.ToString();
        _playerTotalExpText.text = battlerToLoad.BattlerTotalExp.ToString();
        _playerExpToNextLevelText.text = battlerToLoad.BattlerExpToNextLevel.ToString();
        _playerExpSlider.maxValue = battlerToLoad.BattlerExpToNextLevel;
        _playerExpSlider.value = battlerToLoad.BattlerCurrentExpThisLevel;
        _expToGive = expGained;
        _playerImage.sprite = battlerToLoad.BattlerPortrait;
        _playerPortraitColor.color = battlerToLoad.PortraitColor;
        _playerExpGainText.text = _expToGive.ToString();

    }


    public void OnPlayerClickEvent(object obj, EventArgs e)
    {
        if (!IsLoaded)
            return;
        if (!_isInitiated)
        {
            _isInitiated = true;
            StartExpGain(_expToGive);
        }
        else
        {
            defaultWaitTimeInstance = new WaitForSeconds(_fastWaitTime);
            defaultLevelUpInstance = new WaitForSeconds(_fastLevelUpWaitTime);
        }

    }

    private void StartExpGain(int expGained)
    {
        _expGainingCoroutine = StartCoroutine(PlayExpGainCo(expGained));
        expBarColorTween.DORestart();
    }


    private IEnumerator PlayExpGainCo(int expGained)
    {

        _expToGive = expGained;
        while (_expToGive > 0)
        {
            _expToGive--;
            _playerBattleStats.BattlerTotalExp++;
            CheckForLevelUp();
            UpdateExpText();
            yield return defaultWaitTimeInstance;
        }
        expBarColorTween.DOKill();
        _playerExpSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = purpleColor;
        _expGainingCoroutine = null;
        IsFinished = true;


    }

    private void UpdateExpText()
    {
        _playerTotalExpText.text = _playerBattleStats.BattlerTotalExp.ToString();
        _playerExpToNextLevelText.text = _playerBattleStats.BattlerExpToNextLevel.ToString();
        _playerExpGainText.text = _expToGive.ToString();
        _playerExpSlider.value = _playerBattleStats.BattlerCurrentExpThisLevel;
        _playerLevelText.text = _playerBattleStats.BattlerLvl.ToString();

    }

    private void CheckForLevelUp()
    {
        if (_playerBattleStats.BattlerExpToNextLevel > 0) return;
        StartCoroutine(WaitForSecond());
    }

    private IEnumerator WaitForSecond()
    {
        StopCoroutine(_expGainingCoroutine);
        _expGainingCoroutine = null;
        expBarColorTween.DORewind();
        _playerExpSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.yellow;
        expBarColorLevelUpTween.DORestart();
        SoundController.Instance.PlaySfxOneShot(SoundController.Sfx.LevelUp);
        yield return defaultLevelUpInstance;
        _playerBattleStats.BattlerLvl++;
        UpdateExpText();
        expBarColorLevelUpTween.DORewind();
        expBarColorTween.DORewind();
        _playerExpSlider.maxValue = _playerBattleStats.BattlerExpToNextLevel;
        StartExpGain(_expToGive);
    }
}
