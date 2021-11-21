using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// The Controller that handles updating the gui from player events.
/// </summary>
public class DialogController : MonoBehaviour
{

    /// <summary>
    /// Used by the Interaction Component to see if We are in a dialog
    /// </summary>
    public bool InDialog { get; private set; }

    [SerializeField] private TMP_Text _fullTmpTextToUpdate;
    [SerializeField] private DotweenBroadcasterComponent _dialogBoxOpenTween;
    [SerializeField] private DotweenBroadcasterComponent _dialogBoxCloseTween;
    [SerializeField] private float _timeBetweenTypewriterTyping = 0.1f;
    [SerializeField] private DialogButton _choice1TmpText;
    [SerializeField] private DialogButton _choice2TmpText;

    private bool _dialogBoxLoading;
    private int _currentLocationInDialog;
    private Dialog _dialogToDisplay;
    private const string _dialogBoxOpen = "dialogBoxOpen";
    private const string _dialogBoxClosed = "dialogBoxClose";
    private bool _typewriterTyping;
    private Coroutine _currentTypewriterCoroutine;
    private WaitForSeconds _timeBetweenTypewriterTypingWait;
    private int _currentDialogSelection;
    private bool _choiceMade;

    const string votingUri = "https://jrpgapi.supergoon.com/api";
    private string GetVotingData(int choice) => $"{{\"question\": \"debugRoom-doYouLikeIt\",\"value\": {choice}}}";


    /// <summary>
    /// This is called by the dialog buttons.  It updates the selection number and that a choice was made
    /// </summary>
    /// <param name="selection">The number of button that was pressed</param>
    public void HandleDialogButtonPressed(int selection)
    {
        _currentDialogSelection = selection;
        _choiceMade = true;
        StartCoroutine(PostRequest(selection -1));

        _choice1TmpText.gameObject.SetActive(false);
        _choice2TmpText.gameObject.SetActive(false);

        AdvanceDialog();
    }

    private IEnumerator PostRequest(int selection)
    {
        var req = new UnityWebRequest(votingUri, "POST");
        var jsonToSend = new System.Text.UTF8Encoding().GetBytes(GetVotingData(selection));
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            Debug.Log("Received: " + req.downloadHandler.text);
        }

    }


    /// <summary>
    /// Triggers the interaction dialog for interacting with dialogs
    /// </summary>
    /// <param name="dialogToGoThrough">The Scriptable object dialog that will be gone though</param>
    /// <returns>Returns true if the dialog should continue, and false if the dialog is ending</returns>
    public void HandlePlayerRightClick(Dialog dialogToGoThrough)
    {
        if (_dialogBoxLoading)
            return;
        if (!InDialog)
        {
            InitializeDialogInteraction(dialogToGoThrough);
            return;
        }
        if (_typewriterTyping)
        {
            StopTypewriter();
        }
        else
            AdvanceDialog();
    }

    /// <summary>
    /// Register to watch for dialogBox animation completion events. On startup
    /// </summary>
    private void Start()
    {
        _dialogBoxOpenTween.DotweenCompleteEvent += OnDialogBoxOpenCompletedEvent;
        _dialogBoxCloseTween.DotweenCompleteEvent += OnDialogBoxClosedCompletedEvent;
        _timeBetweenTypewriterTypingWait = new WaitForSeconds(_timeBetweenTypewriterTyping);
    }

    private void InitializeDialogInteraction(Dialog dialogToGoThrough)
    {
        InDialog = true;
        _dialogBoxLoading = true;
        _currentLocationInDialog = 0;
        _fullTmpTextToUpdate.gameObject.SetActive(false);
        _fullTmpTextToUpdate.text = dialogToGoThrough.LinesOfDialog[_currentLocationInDialog].Dialog;
        _dialogToDisplay = dialogToGoThrough;
        _choiceMade = false;
        DOTween.Play(_dialogBoxOpen);
    }

    private void AdvanceDialog()
    {
        if (_dialogToDisplay.LinesOfDialog[_currentLocationInDialog].IsChoice && !_choiceMade)
        {
            _choice1TmpText.UpdateButtonText(_dialogToDisplay.LinesOfDialog[_currentLocationInDialog].ChoiceOptions[0]);
            _choice2TmpText.UpdateButtonText(_dialogToDisplay.LinesOfDialog[_currentLocationInDialog].ChoiceOptions[1]);
            _choice1TmpText.gameObject.SetActive(true);
            _choice2TmpText.gameObject.SetActive(true);
            return;
        }
        if (_currentLocationInDialog + 1 >= _dialogToDisplay.LinesOfDialog.Length)
        {
            DOTween.Play(_dialogBoxClosed);
            return;
        }
        _currentLocationInDialog++;
        if (_dialogToDisplay.LinesOfDialog[_currentLocationInDialog].SelectionChoice == 0 || _dialogToDisplay.LinesOfDialog[_currentLocationInDialog].SelectionChoice == _currentDialogSelection)
        {
            _fullTmpTextToUpdate.text = _dialogToDisplay.LinesOfDialog[_currentLocationInDialog].Dialog;
            InitializeTypewriterEffect();
        }
        else
        {
            AdvanceDialog();
        }
    }

    private void EndDialog()
    {
        InDialog = false;
        _currentLocationInDialog = 0;
        DOTween.Rewind(_dialogBoxClosed);
        DOTween.Rewind(_dialogBoxOpen);
    }

    private void OnDialogBoxOpenCompletedEvent(object thing, EventArgs e)
    {
        _dialogBoxLoading = false;
        _fullTmpTextToUpdate.gameObject.SetActive(true);
        InitializeTypewriterEffect();
    }
    private void OnDialogBoxClosedCompletedEvent(object thing, EventArgs e)
    {
        EndDialog();
    }

    private void InitializeTypewriterEffect()
    {
        _typewriterTyping = true;
        _currentTypewriterCoroutine = StartCoroutine(RevealCharacters(_fullTmpTextToUpdate));
    }

    private void StopTypewriter()
    {
        if (!_typewriterTyping)
            return;
        StopCoroutine(_currentTypewriterCoroutine);
        EndTypewriterText();

    }
    /// <summary>
    /// Method revealing the text one character at a time.
    /// </summary>
    /// <returns></returns>
    IEnumerator RevealCharacters(TMP_Text textComponent)
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;
        var totalVisibleCharacters = textInfo.characterCount; // get # of visible character in text object
        var charactersVisible = 0;
        var shouldContinue = true;

        while (shouldContinue)
        {

            if (charactersVisible >= totalVisibleCharacters)
            {
                EndTypewriterText();
                shouldContinue = false;
            }

            textComponent.maxVisibleCharacters = charactersVisible; // How many characters should TextMeshPro display?

            charactersVisible += 1;

            yield return _timeBetweenTypewriterTypingWait;
        }
    }

    /// <summary>
    /// Modifies the values to stop the typewriter
    /// </summary>
    private void EndTypewriterText()
    {
        _currentTypewriterCoroutine = null;
        _typewriterTyping = false;
        _fullTmpTextToUpdate.maxVisibleCharacters = int.MaxValue;
    }
}
