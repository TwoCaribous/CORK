using UnityEngine;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Look")]
public class Look : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        controller.DisplayRoomText();
    }
}
