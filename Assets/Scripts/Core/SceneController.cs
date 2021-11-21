using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneController 
{
    [System.Serializable]
    public enum GameScenesEnum
    {
        DebugRoom,
        Battle1,
    }

    private static readonly Dictionary<GameScenesEnum, string> gameSceneToSceneNameDictionary = new Dictionary<GameScenesEnum, string>
    {
        {GameScenesEnum.DebugRoom, "DebugRoom"},
        {GameScenesEnum.Battle1, "battle1"}
    };

    /// <summary>
    /// Changes the game scene by using the lookup dictionary that maps the scene name to the enum of game scenes
    /// </summary>
    /// <param name="sceneToChangeTo"></param>
    public static void ChangeGameScene(GameScenesEnum sceneToChangeTo)
    {
        SceneManager.LoadScene(gameSceneToSceneNameDictionary[sceneToChangeTo]);
    }


}
