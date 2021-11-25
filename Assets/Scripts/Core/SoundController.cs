using System.Collections.Generic;
using FMOD.Studio;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    [SerializeField]
    private SerializableDictionaryBase<Bgm, string> _bgmMusicDictionary;
    [SerializeField]
    private SerializableDictionaryBase<Sfx, string> _sfxMusicDictionary;

    private readonly Dictionary<Bgm, EventInstance> _loadedBattleMusic = new Dictionary<Bgm, EventInstance>();
    private EventInstance _currentPlayingEventInstance;

    private readonly Dictionary<Sfx, EventInstance> _loadedBattleSfx = new Dictionary<Sfx, EventInstance>();


    private const string _bgmBus = "bus:/bgm";
    /// <summary>
    /// This is used to ninitialize the sound inside of webgl builds.
    /// </summary>
    public static bool IsWebGlSoundInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void StopAllBgm()
    {
        FMODUnity.RuntimeManager.GetBus(_bgmBus).stopAllEvents(STOP_MODE.ALLOWFADEOUT);
        _currentPlayingEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public void LoadBgm(Bgm bgmToPlay)
    {
        if (_loadedBattleMusic.ContainsKey(bgmToPlay))
            return;

        if (_bgmMusicDictionary.TryGetValue(bgmToPlay, out var bgmToPlayString))
        {
            var loadedInstance = FMODUnity.RuntimeManager.CreateInstance(bgmToPlayString);
            _loadedBattleMusic.Add(bgmToPlay, loadedInstance);
            return;
        }
        DebugLogger.SendDebugMessage("Could not load bgm, sending default instance.");
    }

    public void PlayBgm(Bgm bgmToPlay)
    {
        if (!_loadedBattleMusic.TryGetValue(bgmToPlay, out var bgmEventInstance)) return;
        bgmEventInstance.start();
        //bgmEventInstance.release();
        _currentPlayingEventInstance = bgmEventInstance;
    }

    public void StopBgm(Bgm bgmToPlay, bool destroyAfter = true)
    {
        if (!_loadedBattleMusic.TryGetValue(bgmToPlay, out var bgmEventInstance)) return;
        bgmEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        if (destroyAfter)
            _loadedBattleMusic.Remove(bgmToPlay);
    }

    public void LoadSfx(Sfx sfxToLoad)
    {
        if (_loadedBattleSfx.ContainsKey(sfxToLoad))
            return;
        if (_sfxMusicDictionary.TryGetValue(sfxToLoad, out var sfxToLoadString))
        {
            var loadedInstance = FMODUnity.RuntimeManager.CreateInstance(sfxToLoadString);
            _loadedBattleSfx.Add(sfxToLoad, loadedInstance);
            return;
        }
        DebugLogger.SendDebugMessage("Cound not load sfx, sending default");
    }

    public void LoadSfx(IEnumerable<Sfx> sfxToLoadArray)
    {
        foreach (var _sfx in sfxToLoadArray)
        {
            LoadSfx(_sfx);
        }
    }

    public void PlaySfx(Sfx sfxToPlay)
    {
        if(!_loadedBattleSfx.ContainsKey(sfxToPlay))
            LoadSfx(sfxToPlay);
        if(!_loadedBattleSfx.TryGetValue(sfxToPlay, out var sfxEventInstance))
            return;
        sfxEventInstance.start();
    }

    /// <summary>
    /// Probably latency issues with this, only use if needed
    /// </summary>
    /// <param name="sfxToPlay"></param>
    public void PlaySfxOneShot(Sfx sfxToPlay)
    {
        if (_sfxMusicDictionary.TryGetValue(sfxToPlay, out var soundToPlay))
            FMODUnity.RuntimeManager.PlayOneShot(soundToPlay);
    }

    public enum Bgm
    {
        DebugRoom,
        Battle1,
        BattleWin,
        BattleRewardGaining,

    }

    public enum Sfx
    {
        Default = 0,
        BattleEnemyDeath = 1,
        LevelUp = 2,
        Dash = 3,
        SwordSlash = 4,
        PlayerTurn = 5,
        CastMagic,
        Critical,
        Ice,
        Bolt,
    }

}
