
public class BattleMusicHandler 
{
    public static void LoadBattleMusic()
    {
        SoundController.Instance.LoadBgm(SoundController.Bgm.BattleWin);
        SoundController.Instance.LoadBgm(SoundController.Bgm.BattleRewardGaining);
    }

    public static void StopBattleMusic()
    {
        SoundController.Instance.StopAllBgm();
        SoundController.Instance.StopBgm(SoundController.Bgm.Battle1);
    }

    public static void PlayBattleWin()
    {
        SoundController.Instance.PlayBgm(SoundController.Bgm.BattleWin);
    }

    public static void StopBattleWin()
    {
        SoundController.Instance.StopBgm(SoundController.Bgm.BattleWin);
    }
}
