using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Move")]
public class Move : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            List<string> roomNames = controller.roomNavigation.currentRoom.GetConnectedRoomNames();

            if (roomNames == null || roomNames.Count == 0)
                controller.LogStringWithReturn("There are no exits from here.");
            else
                controller.LogStringWithReturn("You can go to: " + string.Join(", ", roomNames) + ".");
        }
        else
        {
            string targetRoom = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);
            controller.roomNavigation.AttemptToChangeRooms(targetRoom);
        }
    }
}
