using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The notifications gui that will show notifications in the GUI for the player such as damage numbers or helping text
/// </summary>
public class BattleNotificationsGui : MonoBehaviour
{
    public void DisplayCanvas(bool isEnabled) => _canvas.enabled = isEnabled;
    [SerializeField] private Canvas _canvas;

    [SerializeField] private Image _backgroundImage;

    [SerializeField] private TMP_Text _pleaseSelectATargetTmp;
    [SerializeField] private TMP_Text _battleNotificationTmp;

    [SerializeField] private GameObject _damageTextToSpawn;
    [SerializeField] private GameObject _damageTextParent;

    [SerializeField] private GameObject _battlerNameDisplayPrefab;
    [SerializeField] private GameObject _battlerNameTextParent;



    private Queue<BattleDamageText> _damageTextQueue;
    private Queue<TMP_Text> _displayNameQueue;


    /// <summary>
    /// This is used to spawn the texts that will be used in battle for displaying damage numbers.
    /// </summary>
    public void SpawnDamageTexts()
    {
        _damageTextQueue = new Queue<BattleDamageText>();
        for (int i = 0; i < 10; i++)
        {
            var instantiatedDamageText = Instantiate(_damageTextToSpawn, _damageTextParent.transform);
            var instantiatedTmpText = instantiatedDamageText.GetComponent<BattleDamageText>();
            instantiatedTmpText.enabled = false;
            _damageTextQueue.Enqueue(instantiatedTmpText);
        }
    }
    /// <summary>
    /// This is used to spawn the texts that will be used in battle for displaying damage numbers.
    /// </summary>
    public void SpawnNameTexts(int numberToSpawn)
    {
        _displayNameQueue = new Queue<TMP_Text>();
        for (int i = 0; i < numberToSpawn; i++)
        {
            var instantiatedDamageText = Instantiate(_battlerNameDisplayPrefab, _battlerNameTextParent.transform);
            var instantiatedTmpText = instantiatedDamageText.GetComponent<TMP_Text>();
            instantiatedTmpText.enabled = false;
            _displayNameQueue.Enqueue(instantiatedTmpText);
        }
    }

    public TMP_Text GetBattlerNameTextFromQueue() => _displayNameQueue.Dequeue();
    public void ReturnBattlerNameFromQueue(TMP_Text returningTmp) => _displayNameQueue.Enqueue(returningTmp);

    /// <summary>
    /// Gets a text from the tmp text queue for pooling
    /// </summary>
    /// <returns>A tmp text that can be used for displaying damage.</returns>
    public BattleDamageText GetTmpTextFromQueue()
    {
        return _damageTextQueue.Dequeue();
    }

    /// <summary>
    /// Puts the tmp text back into the queue
    /// </summary>
    /// <param name="textToReturn"></param>
    public void ReturnDamageTextToQueue(BattleDamageText textToReturn)
    {
        _damageTextQueue.Enqueue(textToReturn);
    }

    /// <summary>
    /// Enables the select a target text on screen
    /// </summary>
    /// <param name="isEnabled"></param>
    public void EnableSelectATarget(bool isEnabled)
    {
        if (_pleaseSelectATargetTmp.enabled == isEnabled)
            return;
        _pleaseSelectATargetTmp.enabled = isEnabled;
    }

    /// <summary>
    /// Shows a text on the screen
    /// </summary>
    /// <param name="textToDisplay"></param>
    public void DisplayBattleNotification(string textToDisplay)
    {
        _battleNotificationTmp.text = textToDisplay;
        _battleNotificationTmp.enabled = true;
        _backgroundImage.enabled = true;

    }

    /// <summary>
    /// Disables a text on the screen;
    /// </summary>
    public void DisableBattleNotification()
    {
        _backgroundImage.enabled = false;
        _battleNotificationTmp.enabled = false;
    }

}

