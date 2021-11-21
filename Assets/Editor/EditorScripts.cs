using UnityEditor;
class EditorScripts
{
    private const string _locationPath = "webgl";
    private const string _debugRoomSceneName = "Assets/Scenes/DebugRoom.unity";
    private const string _battleSceneName = "Assets/Scenes/battle1.unity";
    static void PerformBuild()
    {
        var options = new BuildPlayerOptions
        {
            target = BuildTarget.WebGL,
            scenes = new[] { _debugRoomSceneName, _battleSceneName },
            locationPathName = _locationPath,
            options = BuildOptions.None,
        };
        BuildPipeline.BuildPlayer(options);
    }
}