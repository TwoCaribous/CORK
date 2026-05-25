using UnityEngine;

/// <summary>
/// Handles the 'help' command. Prints the full command reference.
///
/// Create via: Assets > Create > TextAdventure > InputActions > Help
/// Assign to GameController.inputActions[] and set keyWord to "help".
/// </summary>
[CreateAssetMenu(menuName = "TextAdventure/InputActions/Help")]
public class Help : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        controller.LogStringWithReturn(controller.GetCommandHelp());
    }
}
