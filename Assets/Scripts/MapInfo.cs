using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MapInfo : MonoBehaviour
{
    [SerializeField] private SoundController.Bgm bgmToPlay;

    [SerializeField] private SoundController.Bgm[] bgmToLoad;
    void Start()
    {
        foreach (var _bgm in bgmToLoad)
        {
            SoundController.Instance.LoadBgm(_bgm);
        }
        SoundController.Instance.PlayBgm(bgmToPlay);

    }

    private void OnMouseClick()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer) return;
        if (SoundController.IsWebGlSoundInitialized) return;
        var result = FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
        result = FMODUnity.RuntimeManager.CoreSystem.mixerResume();
        SoundController.IsWebGlSoundInitialized = true;
    }
}
