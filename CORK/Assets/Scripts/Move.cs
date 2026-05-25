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
    if (!conn.isHidden && conn.connectedRoom != null)
    {
        bool isUp = string.Equals(conn.direction?.Trim(), "up", System.StringComparison.OrdinalIgnoreCase);

        string line;

        if (conn.hasBeenVisited)
        {
            string prefix = isUp ? "Above you" : "To the " + conn.direction;
            string segment = "(" + conn.connectedRoom.roomName + ") ";
            if (!string.IsNullOrEmpty(conn.doorDescription))
                segment += conn.doorDescription;
            line = prefix + ", " + segment.TrimEnd();
        }
        else
        {
            string prefix = isUp ? "Above you" : "To the " + conn.direction;
            string segment = "";
            if (!string.IsNullOrEmpty(conn.displayName))
                segment += "(" + conn.displayName + ") ";
            if (!string.IsNullOrEmpty(conn.doorDescription))
                segment += conn.doorDescription;
            line = prefix;
            if (!string.IsNullOrEmpty(segment))
                line += ", " + segment.TrimEnd();
        }

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
