using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Rooms;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Move")]
public class Move : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            List<RoomConnection> connections = controller.roomNavigation.currentRoom.connections;
            List<string> exitLines = new List<string>();

            foreach (RoomConnection conn in connections)
            {
                if (!conn.isHidden && conn.connectedRoom != null && !string.IsNullOrEmpty(conn.doorDescription))
                {
                    string line = conn.hasBeenVisited
                        ? "To the " + conn.direction + " (" + conn.connectedRoom.roomName + "), " + conn.doorDescription
                        : "To the " + conn.direction + ", " + conn.doorDescription;
                    exitLines.Add(line);
                }
            }

            if (exitLines.Count == 0)
                controller.LogStringWithReturn("There are no exits from here.");
            else
                controller.LogStringWithReturn(string.Join("\n", exitLines));
        }
        else
        {
            string targetRoom = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);
            controller.roomNavigation.AttemptToChangeRooms(targetRoom);
        }
    }
}
