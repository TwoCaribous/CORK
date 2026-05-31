using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Quit")]
public class QuitCommand : InputAction
{
    [Tooltip("The exact name of your start screen scene as it appears in Build Settings.")]
    public string startSceneName = "StartScreen";

    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        // GameController.OnDestroy() fires automatically when the scene unloads,
        // which restores all ScriptableObject snapshots and clears all flags.
        SceneManager.LoadScene(startSceneName);
    }
}
