using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Attach to a GameObject in the StartScreen scene.
/// Waits for Enter, then loads the next scene.
/// </summary>
public class StartScreenController : MonoBehaviour
{
    [Tooltip("The exact name of the scene to load when Enter is pressed (e.g. IntroScroll or Main).")]
    public string nextSceneName = "IntroScroll";

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(nextSceneName);
        }

        if (kb.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }
}
